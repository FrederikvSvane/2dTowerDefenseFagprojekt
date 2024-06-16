using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sell : MonoBehaviour
{
    [SerializeField] private Tower _tower;
    [SerializeField] private SpriteRenderer _renderer;
    private Color _standardColor;
    [SerializeField] private Color _hoverColor;

    void Start(){
        _standardColor = _renderer.color;
    }
    void OnMouseDown(){
        _tower.TriggerSell();
    }

    void OnMouseEnter(){
        _renderer.color = _hoverColor;
    }   

    void OnMouseExit(){
        _renderer.color = _standardColor;
    }
}
