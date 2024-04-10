using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Tower towerOnTile;
    public GameObject _activeTower;
    public GameObject _startPoint;
    public GameObject _endPoint;
    public GameObject _path;
    public GameObject _cannotSetBlock;
    public bool isWalkable = true;
    private GridManager _gridManager;

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    public void OnMouseDown()
    {
        bool clickedIllegalTile = _endPoint.activeSelf || _startPoint.activeSelf;
        bool enemyOnTile = CheckCollisionWithEnemy();

        if (!clickedIllegalTile)
        {
            if (enemyOnTile)
            {
                ShowWarningIllegalTileClicked();
            }
            else
            {
                bool isTowerOnTile = towerOnTile != null;
                bool canAffordTower = _gridManager.GetPlayer().GetCoinBalance() >= _activeTower.GetComponent<Tower>().getCost();

                if (isTowerOnTile)
                {
                    towerOnTile.Suicide();
                    _gridManager.GetPlayer().SubtractCoinsFromBalance(-towerOnTile.getCost() * 0.4f);
                    isWalkable = true;
                }
                else if (canAffordTower)
                {
                    towerOnTile = Instantiate(_activeTower, transform.position, Quaternion.identity).gameObject.GetComponent<Tower>();
                    float towerPrice = towerOnTile.getCost();
                    Player player = _gridManager.GetPlayer();
                    player.SubtractCoinsFromBalance(towerPrice);
                    isWalkable = false;
                }
                _gridManager.FindAndShowShortestPathOnClick();
            }
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
            if (collider != boxCollider && collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"Enemy on tile at {transform.position}");
                return true;
            }
        }
        return false;
    }

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
        return towerOnTile;
    }

    public void SetActiveTurret()
    {
        //Do soemthing
    }
}