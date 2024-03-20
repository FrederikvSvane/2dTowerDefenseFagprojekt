using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    [SerializeField] public GameObject _SetBlock;

    [SerializeField] public GameObject _startPoint;
    [SerializeField] public GameObject _endPoint;

    [SerializeField] public GameObject _path;

    [SerializeField] public bool isWalkable = true;

        private GridManager _gridManager;

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
    }

    public void OnMouseDown()
    {
        if (_SetBlock.activeSelf)
        {
            _SetBlock.SetActive(false);
            isWalkable = true;
        }
        else
        {
            if (!_endPoint.activeSelf && !_startPoint.activeSelf)
            {
                _SetBlock.SetActive(true);
                isWalkable = false;
            }
        }

        // Call the method to find and show the shortest path
        _gridManager.FindAndShowShortestPathOnClick();
    }
 
    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }
 
    void OnMouseEnter() {
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    public void setTileAsCurrentPath()
    {
        _path.SetActive(true);
    }
}