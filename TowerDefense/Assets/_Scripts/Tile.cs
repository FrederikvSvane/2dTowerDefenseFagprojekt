using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Tile : MonoBehaviour {
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    [SerializeField] private GameObject _SetBlock;

    [SerializeField] public GameObject _startPoint;
    [SerializeField] public GameObject _endPoint;

    [SerializeField] private GameObject _path;

    [SerializeField] public bool isWalkable = true;
 
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

    //When the tile is clicked, the SetBlock object will be toggled on and off
    void OnMouseDown()
    {
        if (_SetBlock.activeSelf)
        {
            _SetBlock.SetActive(false);
        }
        else
        {
            //set the SetBlock object to active if endpoint and startpoint are not active
            if (!_endPoint.activeSelf && !_startPoint.activeSelf)
            {
                _SetBlock.SetActive(true);
            }
        }
    }

    public void setTileAsCurrentPath()
    {
        _path.SetActive(true);
    }
}