using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Unity.Collections;
using UnityEditor;

public class GridManager : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    public GameObject _enemyPrefab;
    [SerializeField] private Transform _cam;
    public Dictionary<Vector2, Tile> _tiles;
    public List<Vector2Int> _path;
    private Vector2Int _start;
    private Vector2Int _end;
    private bool hasPath;
    [SerializeField] public int numberOfEnemiesToSpawn = 10;
    private List<Enemy> enemies = new List<Enemy>();

    public AStarNode[,] aStarNodeGrid;

    public TowerManager _towerManager;

    private int playerCount=2;

    public Player player;


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _tiles = new Dictionary<Vector2, Tile>();
        AssignReferences();
        InitializeGrid();
        _towerManager = FindObjectOfType<TowerManager>();
        if (_towerManager == null) Debug.Log("_towerManager is null");
    }

    // void Start()
    // {
    //     GenerateGrid();
    //     GenerateASTarNodeGrid();
    //     FindAndShowShortestPath();
    //     SpawnEnemies();
    //     Physics2D.IgnoreLayerCollision(7, 3);
    //     initializePlayer();
    // }

    void AssignReferences()
    {
        _tilePrefab = Resources.Load<Tile>("Tile");
        _enemyPrefab = Resources.Load<GameObject>("Enemy");
        _cam = GameObject.FindWithTag("MainCamera").transform;

    }

    void InitializeGrid()
    {
        Dictionary<String, int> playerMap = new Dictionary<String, int>()
        {
            { "7382a819-e10e-4488-a406-b2b91c44a68b", 1 },
            { "7382a819-e10e-4488-a406-b2b91c44a68v", 2 },
            { "500a1a29-14ff-43e8-ab75-65bc6a474b45", 3 }
        };
        GenerateGridDynamicPlacement(playerMap);
        GenerateASTarNodeGridDynamicPosition(playerMap);
        FindAndShowShortestPath();
        //SpawnEnemies();
        Physics2D.IgnoreLayerCollision(7, 3);
        InitializePlayer();
    }
    void InitializePlayer()
    {
        player = FindObjectOfType<Player>();
        player.SetCoinBalance(100);
        player.SetHealth(100);
    }

    void GenerateGridDynamicPlacement(Dictionary<String, int> playerMap){
        // playerMap = {(7382a819-e10e-4488-a406-b2b91c44a68b, 1), (500a1a29-14ff-43e8-ab75-65bc6a474b45, 2), ...} 
        int playerCount = playerMap.Count;
        //playerMap[PhotonNetwork.LocalPlayer.UserId];
        int currentPlayerNumber = 1;
        foreach (var player in playerMap)
        {

                Vector2 gridGenerationStartingPoint = new Vector2(0, (_height*(currentPlayerNumber-1)+currentPlayerNumber-1));
                Debug.Log(gridGenerationStartingPoint);
                GenerateGridFromPoint(gridGenerationStartingPoint);
                currentPlayerNumber++;
            
        }
    }

    void GenerateGridFromPoint(Vector2 startPoint)
    {
        for (int x = (int)startPoint.x ; x < _width+(int)startPoint.x; x++)
        {
            for (int y = (int)startPoint.y; y < _height+(int)startPoint.y; y++)
            {
                Vector2 tilePosition = new Vector2(x + 0.5f, y + 0.5f);
                object[] instantiationData = { x, y }; // Custom instantiation data

                var spawnedTile = PhotonNetwork.Instantiate(_tilePrefab.name, tilePosition, Quaternion.identity, 0, instantiationData);
                Tile tileComponent = spawnedTile.GetComponent<Tile>();
                tileComponent.name = $"Tile({x},{y})";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tileComponent.Init(isOffset);

                _start = new Vector2Int((int)startPoint.x, (int)startPoint.y);
                _end = new Vector2Int((int)startPoint.x + _width - 1, (int)startPoint.y + _height - 1);

                // If tile at position is the start point, activate the start point object
                if (x == _start.x && y == _start.y)
                {
                    tileComponent._startPoint.SetActive(true);
                }

                // If tile at position is the end point, activate the end point object
                if (x == _end.x && y == _end.y)
                {
                    tileComponent._endPoint.SetActive(true);
                }

                _tiles[new Vector2(x, y)] = tileComponent;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    void GenerateASTarNodeGridDynamicPosition(Dictionary<String, int> playerMap)
    {
        aStarNodeGrid = new AStarNode[_width, _height];
        foreach (var player in playerMap)
        {
            Vector2 gridGenerationStartingPoint = new Vector2(0, _height * (player.Value - 1) + player.Value - 1); //æseløre
            GenerateASTarNodeGridFromStartingPoint(gridGenerationStartingPoint);
        }
    }
    

    void GenerateASTarNodeGridFromStartingPoint(Vector2 startingPoint)
    {
        aStarNodeGrid = new AStarNode[_width, _height];
        for (int i = (int)startingPoint.x; i < (int)startingPoint.x+_width; i++)
        {
            for (int j = (int)startingPoint.y; j < (int)startingPoint.y+_height; j++)
            {
                Tile tile = GetTileAtPosition(new Vector2(i, j));
                if (tile == null) {
                    Debug.LogError("Tile is null at position: " + i + ", " + j);
                    continue; // Skip this iteration if tile is null
                }


                aStarNodeGrid[i - (int)startingPoint.x, j-(int)startingPoint.y] = new AStarNode
                {
                    isWalkable = tile._isWalkable,
                    position = new Vector2Int(i, j)
                };
            }
        }
    }

    public void FindAndShowShortestPath()
    {

        _path = AStarPathfinding.FindPath(aStarNodeGrid, _start, _end);

        if (_path != null)
        {
            hasPath = true;
            if (_path.Count == 0)
            {
                hasPath = false;
            }

            WipeCurrentPath();
            SetNewPath();
        }
        else
        {
            hasPath = false;
        }
    }


    public void WipeCurrentPath()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tiles[new Vector2(x, y)]._path.SetActive(false);
            }
        }
    }

    public void SetNewPath()
    {
        int[,] gridPattern = new int[_width, _height];
        // set new path
        foreach (Vector2Int Coord in _path)
        {
            gridPattern[Coord.x, Coord.y] = 2;
        }
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (gridPattern[x, y] == 2)
                {
                    _tiles[new Vector2(x, y)].setTileAsCurrentPath();
                }
            }
        }
    }

    public void FindAndShowShortestPathOnClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));
        Tile tile = GetTileAtPosition(gridPosition);
        aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile._isWalkable;
        if (tile != null)
        {
            FindAndShowShortestPath();
            if (!hasPath)
            {
                tile.getTower().Suicide();
                player.SubtractCoinsFromBalance(-tile.getTower().GetCost());
                tile._isWalkable = true;
                aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile._isWalkable;
                FindAndShowShortestPath();
                ShowWarningIllegalTileClicked(tile);
                return;
            }

            foreach (Enemy enemy in enemies)
            {
                //If the enemy object has been destroyed, remove it from enemies
                if (enemy == null)
                {
                    enemies.Remove(enemy);
                    break;
                }

                enemy.FindPathToEndTile();
                if (!enemy.hasPath)
                {
                    tile.getTower().Suicide();
                    tile._isWalkable = true;
                    aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile._isWalkable;
                    FindAndShowShortestPath();
                    enemy.FindPathToEndTile();
                    ShowWarningIllegalTileClicked(tile);
                    break;
                }
            }
        }
    }

    public void ShowWarningIllegalTileClicked(Tile tile)
    {
        StartCoroutine(ShortlyActivateCannotSetBlock(tile));
    }

    private IEnumerator ShortlyActivateCannotSetBlock(Tile tile)
    {
        tile._cannotSetBlock.SetActive(true); // Activate the GameObject
        yield return new WaitForSeconds(0.1f); // Wait for the specified delay
        tile._cannotSetBlock.SetActive(false); // Deactivate the GameObject
    }


    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    private void SpawnEnemies()
    {   
        StartCoroutine(SpawnEnemy());
        IEnumerator SpawnEnemy()
        {
            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {

                Vector3 spawnPosition = GetTileAtPosition(_start).transform.position;
                GameObject enemyInstance = PhotonNetwork.Instantiate(_enemyPrefab.name, spawnPosition, Quaternion.identity);
                Enemy enemy = enemyInstance.GetComponent<Enemy>();
                enemies.Add(enemy);
                yield return new WaitForSeconds(1f);
            }
        }

    }

    public Player GetPlayer()
    {
        return player;
    }

    public int GetGridStartingPointX()
    {
        return _start.x;
    }

    public int GetGridStartingPointY()
    {
        return _start.y;
    }

    public Vector2Int GetGridStartingPoint()
    {
        return _start;
    }

    public int GetGridEndPointX()
    {
        return _end.x;
    }

    public int GetGridEndPointY()
    {
        return _end.y;
    }

    public Vector2Int GetGridEndPoint()
    {
        return _end;
    }


}