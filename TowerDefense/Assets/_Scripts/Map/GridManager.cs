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
    [SerializeField] private int _width, _height, _spacing;
    [SerializeField] private string _layout = "horizontal"; // "vertical", "horizontal", "grid"
    [SerializeField] private Vector2Int _bottomLeftCornerOfPlayerOne { get; } = new Vector2Int(0, 0);

    [SerializeField] private Tile _tilePrefab;
    public GameObject _enemyPrefab;
    [SerializeField] private Transform _cam;
    public Dictionary<Vector2, Tile> _tiles;
    public List<Vector2Int> _path;
    [SerializeField] public Vector2Int _startRelativeToOwnMap;
    [SerializeField] public Vector2Int _endRelativeToOwnMap;
    public Vector2Int _startRelativeToGlobalGrid { get; private set; }
    public Vector2Int _endRelativeToGlobalGrid { get; private set; }
    private bool hasPath;
    [SerializeField] public int numberOfEnemiesToSpawn = 10;
    private List<Enemy> enemies = new List<Enemy>();

    public AStarNode[,] aStarNodeGrid;

    public TowerManager _towerManager;

    private int _playerCount;

    public Player player;


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _tiles = new Dictionary<Vector2, Tile>();
        AssignReferences();
        InitializeGrid();
        _towerManager = FindObjectOfType<TowerManager>();
        if (_towerManager == null) Debug.Log("_towerManager is null");
    }

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
            { "500a1a29-14ff-43e8-ab75-65bc6a474b45", 3 },
            { "500a1a29-14ff-43e8-ab75-65bc6a474b46", 4 }
        };
        _playerCount = playerMap.Count;
        GenerateGridDynamicPosition(playerMap);
        GenerateASTarNodeGridDynamicPosition(playerMap);
        FindAndShowShortestPath();
        SpawnEnemiesDynamicPosition(playerMap);
        Physics2D.IgnoreLayerCollision(7, 3);
        InitializePlayer();
    }
    void InitializePlayer()
    {
        player = FindObjectOfType<Player>();
        player.SetCoinBalance(1000);
        player.SetHealth(100);
    }

    void GenerateGridDynamicPosition(Dictionary<String, int> playerMap)
    {
        int playerCount = playerMap.Count;
        //playerMap[PhotonNetwork.LocalPlayer.UserId];
        int currentPlayerNumber = 1;
        foreach (var player in playerMap)
        {
            if (player.Value == 2) //for testing. Replace with player.key == PhotonNetwork.LocalPlayer.UserId
            {
                Vector2 bottomLeftCorner = CalculatePlayerPosition(currentPlayerNumber);
                GenerateGridFromPoint(bottomLeftCorner, player.Key);
            }
            currentPlayerNumber++;
        }
    }

    public Vector2Int CalculatePlayerPosition(int playerNumber)
    {
        int x = 0;
        int y = 0;

        switch (_layout.ToLower())
        {
            case "vertical":
                y = _bottomLeftCornerOfPlayerOne.y + (playerNumber - 1) * (_height + _spacing);
                break;

            case "horizontal":
                x = _bottomLeftCornerOfPlayerOne.x + (playerNumber - 1) * (_width + _spacing);
                break;

            case "grid":
                int row = (playerNumber - 1) / 2;
                int col = (playerNumber - 1) % 2;
                x = _bottomLeftCornerOfPlayerOne.x + col * (_width + _spacing);
                y = _bottomLeftCornerOfPlayerOne.y + row * (_height + _spacing);
                break;
        }

        return new Vector2Int(x, y);
    }

    void GenerateGridFromPoint(Vector2 startPoint, string playerID)
    {
        for (int x = (int)startPoint.x; x < _width + (int)startPoint.x; x++)
        {
            for (int y = (int)startPoint.y; y < _height + (int)startPoint.y; y++)
            {
                Vector2 tilePosition = new Vector2(x + 0.5f, y + 0.5f);
                object[] instantiationData = { x, y }; // Custom instantiation data

                var spawnedTile = PhotonNetwork.Instantiate(_tilePrefab.name, tilePosition, Quaternion.identity, 0, instantiationData);
                Tile tileComponent = spawnedTile.GetComponent<Tile>();
                tileComponent.name = $"Tile({x},{y})";
                tileComponent._playerID = playerID;

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tileComponent.Init(isOffset);

                _startRelativeToGlobalGrid = new Vector2Int((int)startPoint.x, (int)startPoint.y);
                _endRelativeToGlobalGrid = new Vector2Int((int)startPoint.x + _width - 1, (int)startPoint.y + _height - 1);

                // If tile at position is the start point, activate the start point object
                if (x == _startRelativeToGlobalGrid.x && y == _startRelativeToGlobalGrid.y)
                {
                    tileComponent._startPoint.SetActive(true);
                }

                // If tile at position is the end point, activate the end point object
                if (x == _endRelativeToGlobalGrid.x && y == _endRelativeToGlobalGrid.y)
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
        foreach (var player in playerMap)
        {
            if (player.Value == 2)
            { //for testing
                Vector2 gridGenerationStartingPoint = CalculatePlayerPosition(player.Value); //æseløre
                GenerateASTarNodeGridFromStartingPoint(gridGenerationStartingPoint);
            }
        }
    }


    void GenerateASTarNodeGridFromStartingPoint(Vector2 startingPoint)
    {
        aStarNodeGrid = new AStarNode[_width, _height];
        for (int i = (int)startingPoint.x; i < (int)startingPoint.x + _width; i++)
        {
            for (int j = (int)startingPoint.y; j < (int)startingPoint.y + _height; j++)
            {
                Tile tile = GetTileAtPosition(new Vector2(i, j));
                if (tile == null)
                {
                    continue; // Skip this iteration if tile is null
                }


                aStarNodeGrid[i - (int)startingPoint.x, j - (int)startingPoint.y] = new AStarNode
                {
                    isWalkable = tile._isWalkable,
                    position = new Vector2Int(i, j)
                };
            }
        }
    }

    public void FindAndShowShortestPath()
    {
        _path = AStarPathfinding.FindPath(aStarNodeGrid, _startRelativeToOwnMap, _endRelativeToOwnMap);

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
                _tiles[new Vector2(x + _startRelativeToGlobalGrid.x, y + _startRelativeToGlobalGrid.y)]._path.SetActive(false);
            }
        }
    }

    public void SetNewPath()
    {
        int[,] gridPattern = new int[_width, _height];
        // set new path
        foreach (Vector2Int Coord in _path)
        {
            gridPattern[Coord.x - _startRelativeToGlobalGrid.x, Coord.y - _startRelativeToGlobalGrid.y] = 2;
        }
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (gridPattern[x, y] == 2)
                {
                    _tiles[new Vector2(x + _startRelativeToGlobalGrid.x, y + _startRelativeToGlobalGrid.y)].setTileAsCurrentPath();
                }
            }
        }
    }

    public void FindAndShowShortestPathOnClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));
        Tile tile = GetTileAtPosition(gridPosition);
        Vector2Int relativePosition = GetRelativePosition(gridPosition);
        aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
        if (tile != null)
        {
            FindAndShowShortestPath();
            if (!hasPath)
            {
                /*
                tile.getTower().Suicide();
                player.SubtractCoinsFromBalance(-tile.getTower().GetCost());*/

                tile.SellTower(1f);

                tile._isWalkable = true;
                aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
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
                    tile.SellTower(1f);
                    tile._isWalkable = true;
                    aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
                    FindAndShowShortestPath();
                    enemy.FindPathToEndTile();
                    ShowWarningIllegalTileClicked(tile);
                    break;
                }
            }
        }
    }

    public Vector2Int GetRelativePosition(Vector2Int globalPosition)
    {
        List<Vector2Int> playerBoardPositions = new List<Vector2Int>(); // Initialize the list
        for (int player = 0; player <= _playerCount; player++)
        {
            playerBoardPositions.Add(CalculatePlayerPosition(player));
        }
        foreach (Vector2Int playerBoardPosition in playerBoardPositions)
        {
            if (globalPosition.x >= playerBoardPosition.x && globalPosition.x < playerBoardPosition.x + _width &&
                globalPosition.y >= playerBoardPosition.y && globalPosition.y < playerBoardPosition.y + _height)
            {
                int relativeX = globalPosition.x - playerBoardPosition.x;
                int relativeY = globalPosition.y - playerBoardPosition.y;
                return new Vector2Int(relativeX, relativeY);
            }
        }
        return Vector2Int.zero; // Return zero if not found, handle appropriately
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

    private void SpawnEnemiesDynamicPosition(Dictionary<String, int> playerMap)
    {
        StartCoroutine(SpawnEnemy());
        IEnumerator SpawnEnemy()
        {
            foreach (var player in playerMap)
            {
                for (int i = 0; i < numberOfEnemiesToSpawn; i++)
                {
                    if (player.Value == 2)
                    {
                        Vector3 spawnPosition = GetTileAtPosition(CalculatePlayerPosition(player.Value)).transform.position;
                        GameObject enemyInstance = PhotonNetwork.Instantiate(_enemyPrefab.name, spawnPosition, Quaternion.identity);
                        Enemy enemy = enemyInstance.GetComponent<Enemy>();
                        enemies.Add(enemy);
                        yield return new WaitForSeconds(1f);
                    }
                }
            }
        }
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Vector2Int GetGridStartingPoint()
    {
        return _startRelativeToGlobalGrid;
    }

    public Vector2Int GetGridEndPoint()
    {
        return _endRelativeToGlobalGrid;
    }
}