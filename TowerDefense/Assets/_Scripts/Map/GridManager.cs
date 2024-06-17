using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GridManager : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    [SerializeField] public int _width, _height, _spacing;
    [SerializeField] private string _layout;
    [SerializeField] private Vector2Int _bottomLeftCornerOfPlayerOne = new Vector2Int(0, 0);
    [SerializeField] private Tile _tilePrefab;
    public GameObject _unitPrefab;
    public GameObject _flyingUnitPrefab;
    [SerializeField] private Transform _cam;
    public Dictionary<Vector2, Tile> _tiles;
    public List<Vector2Int> _path;
    public List<Vector2Int> _flyingPath;
    [SerializeField] public Vector2Int _startRelativeToOwnMap;
    [SerializeField] public Vector2Int _endRelativeToOwnMap;
    public Vector2Int _startRelativeToGlobalGrid { get; private set; }
    public Vector2Int _endRelativeToGlobalGrid { get; private set; }
    private bool _mapHasPath;
    private List<Unit> _units = new List<Unit>();
    [SerializeField] public int _numberOfUnitsToSpawn = 10;
    public AStarNode[,] _aStarNodeGrid;
    public TowerManager _towerManager;
    public PlayerManager _playerManager;
    public PhotonView _photonView;
    private int _playerCount;
    public Player _player;
    private WavesManager _wavesManager;


    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _tiles = new Dictionary<Vector2, Tile>();
        _wavesManager = FindObjectOfType<WavesManager>();
        AssignReferences();
        InitializeGrid();
        _towerManager = FindObjectOfType<TowerManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _playerManager.InitPlayerHealthValues();
        _photonView = GetComponent<PhotonView>();
    }

    void AssignReferences()
    {
        _tilePrefab = Resources.Load<Tile>("Tile");
        _unitPrefab = Resources.Load<GameObject>("Unit");
        _flyingUnitPrefab = Resources.Load<GameObject>("Flying Unit");
        _cam = GameObject.FindWithTag("MainCamera").transform;
    }

    void InitializeGrid()
    {
        Dictionary<int, Photon.Realtime.Player> playerMap = PhotonNetwork.CurrentRoom.Players;
        _playerCount = playerMap.Count;
        //wavesManager.setPlayerMap(playerMap);
        GenerateGridDynamicPosition(playerMap);
        GenerateASTarNodeGridDynamicPosition(playerMap);
        FindAndShowShortestPath();
        _flyingPath = _path;
        //SpawnUnitsOnAllMaps(playerMap);
        _wavesManager.InitializeWaves(this);
        Physics2D.IgnoreLayerCollision(7, 3);
        InitializePlayer();
        Vector3 centerOfPlayerMap = CalculatePlayerPosition(PhotonNetwork.LocalPlayer.ActorNumber).ToVector3();
        _cam.transform.position = new Vector3(centerOfPlayerMap.x + (_width / 2), centerOfPlayerMap.y + (_height / 2), -10);
        Camera.main.orthographicSize = 7;
    }

    void InitializePlayer()
    {
        _player = FindObjectOfType<Player>();
        _player.SetCoinBalance(1000);
        _player.SetHealth(100);
    }

    void GenerateGridDynamicPosition(Dictionary<int, Photon.Realtime.Player> playerMap)
    {
        foreach (var player in playerMap)
        {
            if (player.Value.UserId == PhotonNetwork.LocalPlayer.UserId)
            {
                Vector2 bottomLeftCorner = CalculatePlayerPosition(player.Key);
                GenerateGridFromPoint(bottomLeftCorner);
            }

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
                int row = (playerNumber - 1) % 2;
                int col = (playerNumber - 1) / 2;
                x = _bottomLeftCornerOfPlayerOne.x + col * (_width + _spacing);
                y = _bottomLeftCornerOfPlayerOne.y + row * (_height + _spacing);
                break;
        }

        return new Vector2Int(x, y);
    }

    void GenerateGridFromPoint(Vector2 startPoint)
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

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                tileComponent.Init(isOffset);

                _startRelativeToGlobalGrid = new Vector2Int((int)startPoint.x, (int)startPoint.y);
                _endRelativeToGlobalGrid = new Vector2Int((int)(startPoint.x + _endRelativeToOwnMap.x), (int)(startPoint.y + _endRelativeToOwnMap.y));

                // If tile at position is the start point, activate the start point object
                if (x == _startRelativeToGlobalGrid.x && y == _startRelativeToGlobalGrid.y)
                {
                    tileComponent.CallSetStartPointActive();
                }

                // If tile at position is the end point, activate the end point object
                if (x == _endRelativeToGlobalGrid.x && y == _endRelativeToGlobalGrid.y)
                {
                    tileComponent.CallSetEndPointActive();
                }

                _tiles[new Vector2(x, y)] = tileComponent;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    void GenerateASTarNodeGridDynamicPosition(Dictionary<int, Photon.Realtime.Player> playerMap)
    {
        foreach (var player in playerMap)
        {

            if (player.Value.UserId == PhotonNetwork.LocalPlayer.UserId)
            {
                Vector2 gridGenerationStartingPoint = CalculatePlayerPosition(player.Key);
                GenerateASTarNodeGridFromStartingPoint(gridGenerationStartingPoint);
            }

        }
    }


    void GenerateASTarNodeGridFromStartingPoint(Vector2 startingPoint)
    {
        _aStarNodeGrid = new AStarNode[_width, _height];
        for (int i = (int)startingPoint.x; i < (int)startingPoint.x + _width; i++)
        {
            for (int j = (int)startingPoint.y; j < (int)startingPoint.y + _height; j++)
            {
                Tile tile = GetTileAtPosition(new Vector2(i, j));
                if (tile == null)
                {
                    continue; // Skip this iteration if tile is null
                }


                _aStarNodeGrid[i - (int)startingPoint.x, j - (int)startingPoint.y] = new AStarNode
                {
                    isWalkable = tile._isWalkable,
                    position = new Vector2Int(i, j)
                };
            }
        }
    }

    public void FindAndShowShortestPath()
    {
        _path = AStarPathfinding.FindPath(_aStarNodeGrid, _startRelativeToOwnMap, _endRelativeToOwnMap);

        if (_path != null && _path.Count > 0)
        {
            _mapHasPath = true;
            WipeCurrentPath();
            SetNewPath();
        }
        else
        {
            _mapHasPath = false;
        }
    }


    public void WipeCurrentPath()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 tileKey = new Vector2(x + _startRelativeToGlobalGrid.x, y + _startRelativeToGlobalGrid.y);
                if (_tiles.TryGetValue(tileKey, out Tile tile))
                {
                    tile.RemoveTileAsCurrentPath();
                }
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
                Vector2 tileKey = new Vector2(x + _startRelativeToGlobalGrid.x, y + _startRelativeToGlobalGrid.y);
                if (gridPattern[x, y] == 2)
                {
                    if (_tiles.TryGetValue(tileKey, out Tile tile))
                    {
                        tile.SetTileAsCurrentPath();
                    }
                }
            }
        }
    }

    public void FindAndShowShortestPathOnClick(Tile tile)
    {
        Vector2Int tilePosition = new Vector2Int((int)tile.transform.position.x, (int)tile.transform.position.y);
        Vector2Int relativePosition = GetRelativePosition(tilePosition);
        Debug.Log("1. Relative position: " + relativePosition);
        Debug.Log("2. tile: " + tile);
        Debug.Log("3. tile._isWalkable: " + tile._isWalkable);
        Debug.Log("4. aStarNodeGrid: " + _aStarNodeGrid[relativePosition.x, relativePosition.y]);
        _aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
        if (tile != null)
        {
            FindAndShowShortestPath();
            if (!_mapHasPath)
            {
                tile.SellTower(1f);
                tile._isWalkable = true;
                _aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
                FindAndShowShortestPath();
                ShowWarningIllegalTileClicked(tile);
                return;
            }

            foreach (Unit unit in _units)
            {
                //If the unit object has been destroyed, remove it from units
                if (unit == null)
                {
                    _units.Remove(unit);
                    break;
                }

                unit.FindPathToEndTile();
                Debug.Log("Unit pathfinding called");
                if (!unit._unitHasPath)
                {
                    tile.SellTower(1f);
                    tile._isWalkable = true;
                    _aStarNodeGrid[relativePosition.x, relativePosition.y].isWalkable = tile._isWalkable;
                    FindAndShowShortestPath();
                    unit.FindPathToEndTile();
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


    public Player GetPlayer()
    {
        return _player;
    }

    public void SpawnUnitsOnAllMaps(int playerID, string unit, int numUnits)
    {
        Unit sendUnit = Resources.Load<Unit>(unit);
        StartCoroutine(SpawnUnit(playerID, sendUnit, numUnits));
    }

    public IEnumerator SpawnUnit(int playerId, Unit setUnit, int numUnits)
    {
        Debug.Log("Gridmanager now spawning units for player " + playerId);
        for (int i = 0; i < numUnits; i++)
        {
            Vector3 spawnPosition = GetTileAtPosition(CalculatePlayerPosition(playerId)).transform.position;
            GameObject unitInstance = PhotonNetwork.Instantiate(setUnit.name, spawnPosition, Quaternion.identity);
            Unit unit = unitInstance.GetComponent<Unit>();
            unit.SetHealth(setUnit._health);
            unit.SetDamage(setUnit._damage);
            unit.SetSpeed(setUnit._moveSpeed);
            _units.Add(unit);
            yield return new WaitForSeconds(1f);
        }
    }

    public int GetNextAlivePlayerId(int currentPlayerId)
    {
        var players = PhotonNetwork.CurrentRoom.Players;
        var playerNrs = players.Keys.OrderBy(Nr => Nr).ToList();

        // Find the index of the current player
        int currentIndex = playerNrs.IndexOf(currentPlayerId);

        // Check subsequent players in the list
        for (int i = currentIndex + 1; i < playerNrs.Count; i++)
        {
            if (_playerManager._playerHealthValues[playerNrs[i] - 1] > 0)
            {
                return playerNrs[i];
            }
        }

        // Wrap around and check from the start of the list
        for (int i = 0; i < currentIndex; i++)
        {
            if (_playerManager._playerHealthValues[playerNrs[i] - 1] > 0)
            {
                return playerNrs[i];
            }
        }

        // If no alive player is found, return -1 or handle appropriately
        return -1;
    }

    [PunRPC]
    private void SpawnUnitsOnMyMap(int playerId, string unit, int numUnits)
    {
        Unit sendUnit = Resources.Load<Unit>(unit);
        StartCoroutine(SpawnUnit(playerId, sendUnit, numUnits));
        Debug.Log("Send unit: " + sendUnit.name);
    }
}