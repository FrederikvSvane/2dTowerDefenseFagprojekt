using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    public Dictionary<Vector2, Tile> _tiles;

    [SerializeField] private Vector2Int _start = new Vector2Int(0, 0);

    [SerializeField] private Vector2Int _end = new Vector2Int(9, 9);

    void Start()
    {
        GenerateGrid();
        FindAndShowShortestPath(_tiles, _start, _end);
    }

    //create a function for calling find and show shortest path each time a tile is clicked
    //it needs to find the current placement of the cursor, and then find the tile at that position
    //then it needs to find the shortest path from the start to the end, and show it on the grid
    public void FindAndShowShortestPathOnClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y));
        Tile tile = GetTileAtPosition(gridPosition);
        if (tile != null)
        {
            FindAndShowShortestPath(_tiles, _start, _end);
        }
        else{
        Debug.Log("Tile not found");
        Debug.Log(gridPosition);
        } 
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
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
                spawnedTile.transform.position = new Vector3(x+0.5f, y+0.5f, 0);

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

    /**
     * Params: Dictionary<Vector2, Tile> tiles, Vector2Int start, Vector2Int end
     * The method finds the shortest path from a given start to a given end
     * and shows the path on the grid of tiles
     * by changing the color of the tiles in the path.
     * So it does not draw the entire map with the path, but only the path.
     */
    public void FindAndShowShortestPath(Dictionary<Vector2, Tile> tiles, Vector2Int start, Vector2Int end)
    {
        int[,] gridPattern = new int[_width, _height];

        // Create a grid
        AStarNode[,] grid = new AStarNode[_width, _height];

        // Initialize the grid
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                grid[x, y] = new AStarNode();
                grid[x, y].position = new Vector2Int(x, y);
                grid[x, y].isWalkable = true;
                if (!tiles[new Vector2(x, y)].isWalkable)
                {
                    grid[x, y].isWalkable = false;
                }
            }
        }

        // Perform the A* algorithm
        List<Vector2Int> path = AStarPathfinding.FindPath(grid, start, end);

        // Print the result
        if (path != null)
        {
            // wipe previous path
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    tiles[new Vector2(x, y)]._path.SetActive(false);
                }
            }

            // set new path
            foreach (Vector2Int Coord in path)
            {
                gridPattern[Coord.x, Coord.y] = 2;
            }
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (gridPattern[x, y] == 2)
                    {
                        tiles[new Vector2(x, y)].setTileAsCurrentPath();
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("No path found");
        }
    }

    public AStarNode[,] ConvertToAStarNodes(Tile[,] tiles)
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        AStarNode[,] nodes = new AStarNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new AStarNode
                {
                    isWalkable = true, // tiles[x, y].IsWalkable, 
                                       //TODO implement IsWalkable in Tile in the future, which updates when tower is placed or removed

                    position = new Vector2Int(x, y)
                };
            }
        }

        return nodes;
    }
}