using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour, IPunInstantiateMagicCallback
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
    private TowerManager _towerManager;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiationData = info.photonView.InstantiationData;
        if (instantiationData != null && instantiationData.Length == 2)
        {
            int x = (int)instantiationData[0];
            int y = (int)instantiationData[1];
            // Initialize the Tile object with the coordinates
            // ...
        }
    }

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
        _towerManager = FindObjectOfType<TowerManager>();

        if (_towerManager == null)
        {
            Debug.LogError("TowerManager not found in the scene!");
        }
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene!");
        }
    }

    public void OnMouseDown()
    {
        bool clickedIllegalTile = _endPoint.activeSelf || _startPoint.activeSelf;
        bool enemyOnTile = CheckCollisionWithEnemy();
        _activeTower = _gridManager.GetPlayer().getTower();


        if (clickedIllegalTile || enemyOnTile)
        {
            ShowWarningIllegalTileClicked();
        }
        else
        {
            bool isTowerOnTile = _towerOnTile != null;
            bool isPlayerDraggingTower = _activeTower != null;

            if (isTowerOnTile)
            {
                SellTower(_towerOnTile); // Should be replaced with SelectTower()
            }
            else if (isPlayerDraggingTower)
            {
                BuyTower(_activeTower);
            }
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

            _gridManager.FindAndShowShortestPathOnClick();
        }
        else
        {
            //Show red warning message on screen
            //Play error sound
            Debug.Log("Need more coins!");
            ShowWarningIllegalTileClicked();
        }
    }

    public void SellTower(Tower tower)
    {
        _towerOnTile.Suicide();

        float towerPrice = _towerOnTile.GetCost();
        Player player = _gridManager.GetPlayer();
        player.AddCoinsToBalance(towerPrice * 0.7f);
        _isWalkable = true;

        _gridManager.FindAndShowShortestPathOnClick();
    }

    [PunRPC]
    private void PlaceTower(Tower tower)
    {
        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        PhotonView photonView = GetComponent<PhotonView>();
        string towerType = tower.getPrefab();
        GameObject towerPrefab = _towerManager.GetTowerPrefab(towerType);
        PhotonNetwork.Instantiate(towerPrefab.name, transform.position, Quaternion.identity);
        photonView.RPC("PlaceTowerOnOtherClients", RpcTarget.Others, transform.position, towerType, playerId);
    }

    [PunRPC]
    private void PlaceTowerOnOtherClients(Vector3 tilePosition, string towerType, int placingPlayerId)
    {
        if (_towerManager != null)
        {
            GameObject towerPrefab = _towerManager.GetTowerPrefab(towerType);
            if (towerPrefab != null)
            {
                GameObject towerInstance = PhotonNetwork.Instantiate(towerPrefab.name, tilePosition, Quaternion.identity);
                Tower towerComponent = towerInstance.GetComponent<Tower>();
                _towerOnTile = towerComponent;
            }
            else
            {
                Debug.LogError("Tower prefab not found");
            }
        }
        else
        {
            Debug.LogError("TowerManager not found!");
        }
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
        _highlight.SetActive(true);
    }

    public bool CheckCollisionWithEnemy()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        Vector2 size = new Vector2(boxCollider.size.x, boxCollider.size.y);
        int layerMask = LayerMask.GetMask("Enemy");

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size, 0, layerMask);

        foreach (Collider2D collider in colliders)
        {
            bool isEnemyOnTile = collider != boxCollider && collider.gameObject.CompareTag("Enemy");
            if (isEnemyOnTile)
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

    public void setTileAsCurrentPath()
    {
        _path.SetActive(true);
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