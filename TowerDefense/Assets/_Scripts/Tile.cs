using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
public class Tile : MonoBehaviour {


    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    [SerializeField] public Tower _activeTower;
    [SerializeField] private GameObject _highlightIllegal;

    [SerializeField] public GameObject _startPoint;
    [SerializeField] public GameObject _endPoint;

    [SerializeField] public GameObject _path;
    [SerializeField] public GameObject _cannotSetBlock;

    [SerializeField] public bool isWalkable = true;
    [SerializeField] private Tower towerOnTile;

    private GridManager _gridManager;

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    public void OnMouseDown()
    {
        if (!_endPoint.activeSelf && !_startPoint.activeSelf){
        bool enemyOnTile = CheckCollisionWithEnemy();
        if (enemyOnTile){
            // Show the cannot set block for a short time
            Debug.Log("Cannot set block");
            StartCoroutine(DeactivateCannotSetBlock(_cannotSetBlock));
            IEnumerator DeactivateCannotSetBlock(GameObject cannotSetBlock)
            {
                cannotSetBlock.SetActive(true); // Activate the GameObject
                yield return new WaitForSeconds(0.1f); // Wait for the specified delay
                cannotSetBlock.SetActive(false); // Deactivate the GameObject
            }
        } else{
            if (!_endPoint.activeSelf && !_startPoint.activeSelf){
                if (towerOnTile != null)
                {
                    //_activeTower.SetActive(false);
                    towerOnTile.Suicide();
                    _gridManager.getPlayer().buyTower(-towerOnTile.getCost() * 0.4f);
                    isWalkable = true;
                }
                else if ( _activeTower.getCost() <=_gridManager.getPlayer().getCoinBalance())
                {   
                    //towerOnTile = Instantiate(_activeTower, transform.position, Quaternion.identity).gameObject.GetComponent<Tower>();
                    towerOnTile = _activeTower.buyTower(_gridManager.getPlayer(), transform);
                    //_gridManager.getPlayer().buyTower(towerOnTile.getCost());
                    isWalkable = false;
                }
        }
        // Call the method to find and show the shortest path
        _gridManager.FindAndShowShortestPathOnClick();
        }

        }
    }
 
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
 
    void OnMouseEnter() {
            _highlight.SetActive(true);
            
    }

public bool CheckCollisionWithEnemy() {
    BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
    Vector2 size = new Vector2(boxCollider.size.x, boxCollider.size.y);
    int layerMask = LayerMask.GetMask("Enemy"); 

    Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size, 0, layerMask);

    foreach (Collider2D collider in colliders) {
        if (collider != boxCollider && collider.gameObject.CompareTag("Enemy")) {
            Debug.Log($"Enemy on tile at {transform.position}");
            return true;
        }
    }
    return false;
}

    // Visualize the collision check area in the Scene view
    void OnDrawGizmos() {
        // Ensure there is a box collider to draw
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null) {
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

    public Tower getTower(){
        return towerOnTile;
    }

    public void SetActiveTurret(){
        //Do soemthing
    }
}