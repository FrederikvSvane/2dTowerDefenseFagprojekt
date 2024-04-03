using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _hitColor;
    [SerializeField] private SpriteRenderer _renderer;
    public float moveSpeed = 0.5f;
    private Vector2Int currentTilePosition;
    private Vector2Int targetTilePosition;
    private GridManager gridManager;
    private List<Vector2Int> path;
    private int currentPathIndex;
    private bool isFollowingGlobalPath = true;

    [Header("Attributes")]
    [SerializeField] private float health = 100f;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>();
        InitializeEnemy();
    }

    private void InitializeEnemy()
    {
        // Set color and start position of the enemy
        _renderer.color = _baseColor;
        transform.position = gridManager.GetTileAtPosition(gridManager._start).transform.position;

        path = gridManager._path;
        currentPathIndex = 0;
        currentTilePosition = gridManager._start;
        setNextTargetTile();
    }

    private void Update()
    {
        if (isFollowingGlobalPath && !IsOnGlobalPath())
        {
            // Enemy has strayed off the global path, find a new path to the end tile
            isFollowingGlobalPath = false;
            FindPathToEndTile();
        }

        moveTowardTargetTile();

        if (currentTilePosition == gridManager._end)
        {
            // Enemy has reached the end tile
            Destroy(gameObject);
        }
    }

    private bool IsOnGlobalPath()
    {
        return path.Contains(currentTilePosition);
    }

    private void FindPathToEndTile()
    {
        AStarNode[,] grid = gridManager.convertTileMapToAStarNodes();
        path = AStarPathfinding.FindPath(grid, currentTilePosition, gridManager._end);
        currentPathIndex = 0;
        setNextTargetTile();
    }

    private void setNextTargetTile()
    {
        if (currentPathIndex < path.Count)
        {
            targetTilePosition = path[currentPathIndex];
            currentPathIndex++;
        }
    }

    private void moveTowardTargetTile()
    {
        Vector3 targetPosition = gridManager.GetTileAtPosition(targetTilePosition).transform.position;
        Tile nextTile = gridManager.GetTileAtPosition(targetTilePosition);

        if (nextTile != null && nextTile.isWalkable)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                currentTilePosition = targetTilePosition;

                if (isFollowingGlobalPath)
                {
                    setNextTargetTile();
                }
                else if (currentTilePosition == gridManager._end || IsOnGlobalPath())
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

    public bool IsOnIndividualPath(Vector2Int position)
    {
        return path.Contains(position);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("I took damage");
        health -= damage;
        _renderer.color = _hitColor;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public float getHealth(){
        return health;
    }

}