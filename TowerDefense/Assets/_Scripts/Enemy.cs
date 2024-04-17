using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _hitColor;
    [SerializeField] private SpriteRenderer _renderer;
    public float moveSpeed = 5.0f;
    private Vector2Int currentTilePosition;
    private Vector2Int targetTilePosition;
    private GridManager gridManager;
    private List<Vector2Int> path;

    private float distanceFromEnd;

    private float onKillValue= 70;
    public bool hasPath = true;
    private int currentPathIndex;
    private bool isFollowingGlobalPath = true;

    [Header("Attributes")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 20f;

    public virtual void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>();
        InitializeEnemy(); 
    }

    // get the distance from the end tile
    public float getDistanceFromEnd()
    {
        return distanceFromEnd;
    }
    private void InitializeEnemy()
    {
        // Set color and start position of the enemy
        _renderer.color = _baseColor;
        transform.position = gridManager.GetTileAtPosition(gridManager.GetGridStartingPoint()).transform.position;

        path = gridManager._path;
        currentPathIndex = 0;
        currentTilePosition = gridManager.GetGridStartingPoint();
        setNextTargetTile();
    }

    public float getOnKillValue()
    {
        return onKillValue;
    }
    public virtual void Update()
    {
        if (isFollowingGlobalPath && !IsOnGlobalPath())
        {
            // Enemy has strayed off the global path, find a new path to the end tile
            isFollowingGlobalPath = false;
            FindPathToEndTile();
        }

        moveTowardTargetTile();

        //Calculate distance to end tile
        
        distanceFromEnd = 0;
        for (int i = currentPathIndex; i < path.Count - 1; i++)
        {
            distanceFromEnd += Vector2.Distance(path[i], path[i + 1]);
        }


        if (currentTilePosition == gridManager.GetGridEndPoint())
        {
            // Enemy has reached the end tile
            gridManager.GetPlayer().SubtractHealthFromBalance(damage);
            Destroy(gameObject);
        }
    }

    private bool IsOnGlobalPath()
    {
        return path.Contains(currentTilePosition);
    }

    public void FindPathToEndTile()
    {
        path = AStarPathfinding.FindPath(gridManager.aStarNodeGrid, currentTilePosition, gridManager.GetGridEndPoint());
        hasPath = path != null && path.Count > 0;
        currentPathIndex = 0;
        setNextTargetTile();
    }

    private void setNextTargetTile()
    {
        if (currentPathIndex < path.Count)
        {
            targetTilePosition = path[currentPathIndex];
            currentPathIndex++;
        } else
        { if (currentTilePosition != gridManager.GetGridEndPoint()){
            hasPath = false;
        }
            
        }
    }

    private void moveTowardTargetTile()
    {
        Vector3 targetPosition = gridManager.GetTileAtPosition(targetTilePosition).transform.position;
        Tile nextTile = gridManager.GetTileAtPosition(targetTilePosition);

        if (nextTile != null && nextTile._isWalkable)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                currentTilePosition = targetTilePosition;

                if (isFollowingGlobalPath)
                {
                    setNextTargetTile();
                }
                else if (currentTilePosition == gridManager.GetGridEndPoint() || IsOnGlobalPath())
                {
                    isFollowingGlobalPath = true;
                    path = gridManager._path;
                    currentPathIndex = path.IndexOf(currentTilePosition) + 1;
                    setNextTargetTile();
                }
                else
                {
                    setNextTargetTile();
                }
            }
        }
        else
        {
            // Next tile is not walkable, find a new path to the end tile
            FindPathToEndTile();
        }
    }

    // Draw the circle collider in the editor
    void OnDrawGizmos() {
        // Ensure there is a box collider to draw
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null) {
            Gizmos.color = Color.red;
            // Convert local circleCollider.radius to world space (assuming no scaling in the transform hierarchy)
            float worldRadius = circleCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
            Vector3 worldCenter = transform.position + new Vector3(circleCollider.offset.x, circleCollider.offset.y, 0);
            // Draw the circle
            Gizmos.DrawWireSphere(worldCenter, worldRadius);
        }
    }

    public bool IsOnIndividualPath(Vector2Int position)
    {
        return path.Contains(position);
    }

    public void TakeDamage(float damage)
    {
        //Debug.Log("I took damage");
        health -= damage;
        _renderer.color = _hitColor;

        if (health <= 0)
        {
            gridManager.GetPlayer().getCoinFromEnemyKill(this);
            Destroy(gameObject);
        }
    }

    public float getHealth(){
        return health;
    }

    public void setHealth(float health){
        this.health = health;
    }

    public float getSpeed(){
        return moveSpeed;
    }

    public void setSpeed(float speed){
        this.moveSpeed = speed;
    }





}