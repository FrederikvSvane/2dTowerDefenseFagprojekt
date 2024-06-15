using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Tower _towerOnTile;
    public Tower _activeTower;
    public GameObject _startPoint;
    public GameObject _endPoint;
    public GameObject _path;
    public GameObject _cannotSetBlock;
    public bool _isWalkable = true;
    private GridManager _gridManager;
    private Player _player;
    private TowerManager _towerManager;
    private PhotonView _photonView;
    [SerializeField] private TowerInformation _towerInformation;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length == 2)
        {
            int x = (int)instantiationData[0];
            int y = (int)instantiationData[1];

            var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
            Init(isOffset);
        }
    }

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
        _towerManager = FindObjectOfType<TowerManager>();
        _photonView = GetComponent<PhotonView>();
        _player = FindObjectOfType<Player>();
        _towerInformation = FindObjectOfType<TowerInformation>(true);
    }

    public void OnMouseDown()
    {
        if (!PauseMenu.GameIsPaused && _photonView.IsMine)
        {
            bool clickedIllegalTile = _endPoint.activeSelf || _startPoint.activeSelf;
            bool unitOnTile = CheckCollisionWithUnit();
            _activeTower = _gridManager.GetPlayer().getTower();


            if (clickedIllegalTile || unitOnTile)
            {
                ShowWarningIllegalTileClicked();
            }
            else
            {
                bool isTowerOnTile = _towerOnTile != null;
                bool isPlayerDraggingTower = _activeTower != null;
                bool isPlayerDragHolding = _activeTower != null; //TODO 

                if (isTowerOnTile)
                {
                    if (_player.GetSelectedTower() != null){
                        _player.GetSelectedTower().ToggleSellOrUpgradeMenu(false);
                    }
                    _player.SetSelectedTower(_towerOnTile);
                    _player.GetSelectedTower().ToggleSellOrUpgradeMenu(true);
                    _towerInformation.transform.gameObject.SetActive(true);
                    return;
                    // SellTower(0.7f); // Should be replaced with SelectTower()
                    // _gridManager.FindAndShowShortestPathOnClick();
                }
                else if (isPlayerDraggingTower)
                {
                    if(_player.GetSelectedTower() != null){
                        _player.GetSelectedTower().ToggleSellOrUpgradeMenu(false);
                    }
                    BuyTower(_activeTower);
                    _gridManager.FindAndShowShortestPathOnClick();
                    _player.SetTower(null);
                } else if (isPlayerDragHolding){

                }
            }
            _towerInformation.transform.gameObject.SetActive(false);
            if (_player.GetSelectedTower() != null)
                _player.GetSelectedTower().ToggleSellOrUpgradeMenu(false);
        }

    }

    public void BuyTower(Tower tower)
    {
        float towerPrice = tower.GetCost();
        float playerCoinBalance = _gridManager.GetPlayer().GetCoinBalance();
        bool canAffordTower = playerCoinBalance >= towerPrice;

        if (canAffordTower)
        {
            PlaceTower(tower);

            Player player = _gridManager.GetPlayer();
            player.SubtractCoinsFromBalance(towerPrice);
            _isWalkable = false;
        }
        else
        {
            //Show red warning message on screen
            //Play error sound
            ShowWarningIllegalTileClicked();
        }
    }

    public void SellTower(float moneyBackRatio)
    {
        //_towerOnTile.Suicide();

        float towerPrice = _towerOnTile.GetCost();
        Player player = _gridManager.GetPlayer();
        player.AddCoinsToBalance(towerPrice * moneyBackRatio);
        _isWalkable = true;

        //Remove the tower from the photon network:
        PhotonNetwork.Destroy(_towerOnTile.gameObject);
        //Update the photonView:
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("RemoveTowerOnAllClients", RpcTarget.All, transform.position);

    }

    // method RemoveTowerOnAllClients for SellTower:
    [PunRPC]
    public void RemoveTowerOnAllClients(Vector3 towerPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(towerPosition, 0.1f, LayerMask.GetMask("Tower"));
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Tower"))
            {
                PhotonNetwork.Destroy(collider.gameObject);
                break;
            }
        }
    }




    //Place a tower on all clients
    [PunRPC]
    private void PlaceTower(Tower tower)
    {
        string towerType = tower.getPrefab();
        GameObject towerPrefab = _towerManager.GetTowerPrefab(towerType);
        GameObject towerInstance = PhotonNetwork.Instantiate(towerPrefab.name, transform.position, Quaternion.identity);
        Tower towerComponent = towerInstance.GetComponent<Tower>();
        towerComponent.SetTile(this);
        _towerOnTile = towerComponent;
    }

    public void ShowWarningIllegalTileClicked()
    {
        StartCoroutine(ShortlyActivateCannotSetBlock());
    }

    private IEnumerator ShortlyActivateCannotSetBlock()
    {
        _cannotSetBlock.SetActive(true); // Activate the GameObject
        yield return new WaitForSeconds(0.1f); // Wait for the specified delay
        _cannotSetBlock.SetActive(false); // Deactivate the GameObject
    }

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {
        if (_photonView.IsMine)
            _highlight.SetActive(true);
    }

    public bool CheckCollisionWithUnit()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = new Vector2(boxCollider.size.x, boxCollider.size.y);
        int layerMask = LayerMask.GetMask("Unit");

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size, 0, layerMask);

        foreach (Collider2D collider in colliders)
        {
            bool isUnitOnTile = collider != boxCollider && collider.gameObject.CompareTag("Unit");
            if (isUnitOnTile)
            {
                return true;
            }
        }
        return false;
    }


    // Draw the box collider in the editor. ##FOR TESTING PURPOSES ONLY##
    void OnDrawGizmos()
    {
        // Ensure there is a box collider to draw
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            Gizmos.color = Color.red;
            // Convert local boxCollider.size to world space (assuming no scaling in the transform hierarchy)
            Vector3 worldSize = boxCollider.size;
            Vector3 worldCenter = transform.position + new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0);
            // Draw the box
            Gizmos.DrawWireCube(worldCenter, new Vector3(worldSize.x, worldSize.y, 0));
        }
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    [PunRPC]
    public void SetTileAsCurrentPath()
    {
        _path.SetActive(true);
    }

    [PunRPC]
    public void RemoveTileAsCurrentPath()
    {
        _path.SetActive(false);
    }

    [PunRPC]
    public void SetStartPointActive()
    {
        _startPoint.SetActive(true);
    }

    [PunRPC]
    public void SetEndPointActive()
    {
        _endPoint.SetActive(true);
    }

    public void CallSetTileAsCurrentPath()
    {
        photonView.RPC("SetTileAsCurrentPath", RpcTarget.AllBuffered);
    }

    public void CallRemoveTileAsCurrentPath()
    {
        photonView.RPC("RemoveTileAsCurrentPath", RpcTarget.AllBuffered);
    }

    public void CallSetStartPointActive()
    {
        photonView.RPC("SetStartPointActive", RpcTarget.AllBuffered);
    }

    public void CallSetEndPointActive()
    {
        photonView.RPC("SetEndPointActive", RpcTarget.AllBuffered);
    }

    public Tower getTower()
    {
        return _towerOnTile;
    }

    public void SetActiveTurret()
    {
        //Do soemthing
    }
}