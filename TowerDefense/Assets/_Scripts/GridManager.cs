using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    public GameObject _enemyPrefab;
    [SerializeField] private Transform _cam;
    public Dictionary<Vector2, Tile> _tiles;
    public List<Vector2Int> _path;
    [SerializeField] public Vector2Int _start;
    [SerializeField] public Vector2Int _end;
    private bool hasPath;
    [SerializeField] public int numberOfEnemiesToSpawn = 10;
    private List<Enemy> enemies = new List<Enemy>();

    public AStarNode[,] aStarNodeGrid;

    public Player player;

    void Start()
    {
        GenerateGrid();
        GenerateASTarNodeGrid();
        FindAndShowShortestPath();
        SpawnEnemies();
        Physics2D.IgnoreLayerCollision(7, 3);
        initializePlayer();
    }

    void initializePlayer()
    {
        player = FindObjectOfType<Player>();
        player.setCoinBalance(100);
        player.setHealth(100);
    }
    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);

                // If tile at position is the start point, activate the start point object
                if (x == _start.x && y == _start.y)
                {
                    spawnedTile._startPoint.SetActive(true);
                }

                // If tile at position is the end point, activate the end point object
                if (x == _end.x && y == _end.y)
                {
                    spawnedTile._endPoint.SetActive(true);
                }

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);

    }

    void GenerateASTarNodeGrid()
    {
        aStarNodeGrid = new AStarNode[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = GetTileAtPosition(new Vector2Int(x, y));
                aStarNodeGrid[x, y] = new AStarNode
                {
                    isWalkable = tile.isWalkable,
                    position = new Vector2Int(x, y)
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

            wipeCurrentPath();
            setNewPath();
        }
        else
        {
            hasPath = false;
        }
    }


    public void wipeCurrentPath()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tiles[new Vector2(x, y)]._path.SetActive(false);
            }
        }
    }

    public void setNewPath()
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
        aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile.isWalkable;
        if (tile != null)
        {
            FindAndShowShortestPath();
            if (!hasPath)
            {
                Debug.Log("No path found");
                tile.getTower().Suicide();
                player.buyTower(-tile.getTower().getCost());
                tile.isWalkable = true;
                aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile.isWalkable;
                FindAndShowShortestPath();
                ShowBlockError(tile);
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
                    tile.isWalkable = true;
                    aStarNodeGrid[gridPosition.x, gridPosition.y].isWalkable = tile.isWalkable;
                    FindAndShowShortestPath();
                    enemy.FindPathToEndTile();
                    ShowBlockError(tile);
                    break;
                }
            }
        }
    }

    public void ShowBlockError(Tile tile)
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

                GameObject enemyInstance = Instantiate(_enemyPrefab, GetTileAtPosition(_start).transform.position, Quaternion.identity);
                Enemy enemy = enemyInstance.GetComponent<Enemy>();
                enemies.Add(enemy);
                yield return new WaitForSeconds(1f);
            }
        }

    }

    public Player getPlayer()
    {
        return player;
    }


}