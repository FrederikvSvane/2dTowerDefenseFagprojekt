using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using WebSocketSharp;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _hitColor;
    [SerializeField] private SpriteRenderer _renderer;
    public PhotonView _photonView;
    public float moveSpeed = 5.0f;
    private Vector2Int currentTilePosition;
    private Vector2Int targetTilePosition;
    private GridManager gridManager;
    private List<Vector2Int> path;

    private float distanceFromEnd;

    private float onKillValue = 70;
    public bool hasPath = true;
    private int currentPathIndex;
    private bool isFollowingGlobalPath = true;
    public string _playerID;

    [Header("Attributes")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 20f;

    public virtual void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _photonView = GetComponent<PhotonView>();
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
        bool hasStrayedOffPath = !IsOnGlobalPath() && isFollowingGlobalPath;
        if (hasStrayedOffPath)
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

        foreach (var playerNr in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            Vector2Int endTile = gridManager.CalculatePlayerPosition(playerNr) + gridManager._endRelativeToOwnMap; //This assumes that all players have the same end tile relative to their own map
            Vector2Int currentUnitPosition = transform.position.ToVector2Int();
            if (playerNr == 2)
            {
                Debug.Log("PlayerNr: " + playerNr + " EndTile: " + endTile);
                Debug.Log("currentUnitPosition: " + currentUnitPosition);
            }
            if (currentUnitPosition == endTile)
            {
                // Enemy has reached the end tile
                gridManager.GetPlayer().SubtractHealthFromBalance(damage);
                Destroy(gameObject);
            }
        }
    }

    private bool IsOnGlobalPath()
    {
        return path.Contains(currentTilePosition);
    }

    public void FindPathToEndTile()
    {
        Vector2Int currrentPosVec = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Vector2Int currentPositionRealativeToOwnMap = gridManager.GetRelativePosition(currrentPosVec);
        path = AStarPathfinding.FindPath(gridManager.aStarNodeGrid, currentPositionRealativeToOwnMap, gridManager._endRelativeToOwnMap);
        hasPath = path != null && path.Count > 0;
        currentPathIndex = 0;
        setNextTargetTile();
    }

    private void setNextTargetTile()
    {
        if (_photonView.IsMine)
        {
            if (currentPathIndex < path.Count)
            {
                targetTilePosition = path[currentPathIndex];
                currentPathIndex++;
            }
            else if (currentTilePosition != gridManager._endRelativeToGlobalGrid)
            {
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
    void OnDrawGizmos()
    {
        // Ensure there is a box collider to draw
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
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
        health -= damage;
        _renderer.color = _hitColor;

        if (health <= 0)
        {
            gridManager.GetPlayer().getCoinFromEnemyKill(this);
            Destroy(gameObject);
        }
    }

    public float getHealth()
    {
        return health;
    }

    public void setHealth(float health)
    {
        this.health = health;
    }

    public float getSpeed()
    {
        return moveSpeed;
    }

    public void setSpeed(float speed)
    {
        this.moveSpeed = speed;
    }





}