using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using WebSocketSharp;

public class Unit : MonoBehaviour
{
    [SerializeField] private Color _hitColor;
    private Color _baseColor;
    [SerializeField] private SpriteRenderer _renderer;
    public PhotonView _photonView;
    public float _moveSpeed = 2.0f;
    private Vector2Int _currentTilePosition;
    private Vector2Int _targetTilePosition;
    public GridManager _gridManager;
    public List<Vector2Int> _path;
    private float _zigZagDistanceFromEnd;
    private float _onKillValue = 70;
    public bool _unitHasPath = true;
    private int _currentPathIndex;
    private bool _isFollowingGlobalPath = true;

    [Header("Attributes")]
    [SerializeField] public float _health;
    [SerializeField] public float _damage;
    public bool _isFlying;
    public Unit(float health, float damage, float speed)
    {
        this._health = health;
        this._damage = damage;
        this._moveSpeed = speed;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _renderer = GetComponent<SpriteRenderer>();
        _baseColor = _renderer.color;
        _photonView = GetComponent<PhotonView>();
        _gridManager = FindObjectOfType<GridManager>();
        InitializeUnit();
        _isFlying = false;
    }

    public virtual void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _baseColor = _renderer.color;
        _photonView = GetComponent<PhotonView>();
        _gridManager = FindObjectOfType<GridManager>();
        InitializeUnit();
        _isFlying = false;
    }

    // get the distance from the end tile
    public float GetDistanceFromEnd()
    {
        return _zigZagDistanceFromEnd;
    }
    private void InitializeUnit()
    {
        // Set color and start position of the unit
        if (_isFlying){
            _path = _gridManager._flyingPath;
        } else {
            _path = _gridManager._path;
        }
        _currentPathIndex = 0;
        _currentTilePosition = _gridManager._startRelativeToGlobalGrid;
        SetNextTargetTile();
    }

    public float GetOnKillValue()
    {
        return _onKillValue;
    }
    public virtual void Update()
    {
        if (_photonView.IsMine)
        {
            bool hasStrayedOffPath = !IsOnGlobalPath() && _isFollowingGlobalPath;
            if (hasStrayedOffPath)
            {
                // Unit has strayed off the global path, find a new path to the end tile
                _isFollowingGlobalPath = false;
                FindPathToEndTile();
            }
            MoveTowardTargetTile();

            //Calculate the zigzag distance to end tile
            _zigZagDistanceFromEnd = 0;
            for (int i = _currentPathIndex; i < _path.Count - 1; i++)
            {
                _zigZagDistanceFromEnd += Vector2.Distance(_path[i], _path[i + 1]);
            }

            int directDistanceFromEnd = (int)Vector2.Distance(_currentTilePosition, _gridManager._endRelativeToGlobalGrid);
            if(directDistanceFromEnd < 0.1f)
            {
                _gridManager.GetPlayer().SubtractHealthFromBalance(_damage);
                RemoveThisUnitOnAllClients();
            }
        }
    }

    [PunRPC]
    public void RemoveThisUnitOnAllClients()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private bool IsOnGlobalPath()
    {
        return _path.Contains(_currentTilePosition);
    }

    public virtual void FindPathToEndTile()
    {
        if(!_isFlying){
            Vector2Int currrentPosVec = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            Vector2Int currentPositionRealativeToOwnMap = _gridManager.GetRelativePosition(currrentPosVec);
            _path = AStarPathfinding.FindPath(_gridManager._aStarNodeGrid, currentPositionRealativeToOwnMap, _gridManager._endRelativeToOwnMap);
            _currentPathIndex = 0;
        }
        _unitHasPath = _path != null;
        SetNextTargetTile();
    }

    public void SetNextTargetTile()
    {
        if (_photonView.IsMine)
        {
            if (_currentPathIndex < _path.Count)
            {
                _targetTilePosition = _path[_currentPathIndex];
                _currentPathIndex++;
            }
            else if (_currentTilePosition != _gridManager._endRelativeToGlobalGrid)
            {
                _unitHasPath = false;
            }
        }
    }

    private void MoveTowardTargetTile()
    {
        Vector3 targetPosition = _gridManager.GetTileAtPosition(_targetTilePosition).transform.position;
        Tile nextTile = _gridManager.GetTileAtPosition(_targetTilePosition);

        if (nextTile != null && (nextTile._isWalkable || _isFlying))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                _currentTilePosition = _targetTilePosition;

                if (_isFollowingGlobalPath)
                {
                    SetNextTargetTile();
                }
                else if (_currentTilePosition == _gridManager._endRelativeToGlobalGrid || IsOnGlobalPath())
                {
                    _isFollowingGlobalPath = true;
                    if(_isFlying)
                    {
                        _path = _gridManager._flyingPath;
                    }
                    else
                    {
                        _path = _gridManager._path;
                    }
                    _currentPathIndex = _path.IndexOf(_currentTilePosition) + 1;
                    SetNextTargetTile();
                }
                else
                {
                    SetNextTargetTile();
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
        return _path.Contains(position);
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _renderer.color = _hitColor;

        if (_health <= 0)
        {
            _gridManager.GetPlayer().getCoinFromUnitKill(this);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Slow(){
        StartCoroutine(ApplySlow());
    }

    IEnumerator ApplySlow(){
        float originalSpeed = _moveSpeed;
        _moveSpeed *= 0.4f;
        yield return new WaitForSeconds(2);
        _moveSpeed = originalSpeed;
    }

    public float GetHealth()
    {
        return _health;
    }

    public void SetHealth(float health)
    {
        this._health = health;
    }

    public float GetSpeed()
    {
        return _moveSpeed;
    }

    public void SetSpeed(float speed)
    {
        this._moveSpeed = speed;
    }

    public void SetDamage(float damage)
    {
        this._damage = damage;
    }

    public bool GetIsFlying()
    {
        return _isFlying;
    }
}