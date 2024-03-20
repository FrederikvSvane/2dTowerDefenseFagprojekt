using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;
 
    [SerializeField] private Tile _tilePrefab;
 
    [SerializeField] private Transform _cam;

    [SerializeField] private Vector2 _startPoint;
    [SerializeField] private Vector2 _endPoint;
 
    private Dictionary<Vector2, Tile> _tiles;
 
    void Start() {
        GenerateGrid();
    }
 
    void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
 
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
                // If tile at position is the start point, activate the start point object
                if (x == _startPoint.x && y == _startPoint.y)
                {
                    spawnedTile._startPoint.SetActive(true);
                }

                // If tile at position is the end point, activate the end point object
                if (x == _endPoint.x && y == _endPoint.y)
                {
                    spawnedTile._endPoint.SetActive(true);
                }
 
 
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
 
        _cam.transform.position = new Vector3((float)_width/2 -0.5f, (float)_height / 2 - 0.5f,-10);

    }
 
    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }


}