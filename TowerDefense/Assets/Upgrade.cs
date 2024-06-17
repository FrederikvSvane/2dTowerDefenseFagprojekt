using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private Tower _tower;
    [SerializeField] private SpriteRenderer _renderer;
    private Color _standardColor;
    [SerializeField] private Color _hoverColor;
    [SerializeField] private bool _isUpgrading;

    void Start(){
        _standardColor = _renderer.color;
    }

    void OnMouseDown(){
        _tower.TriggerUpgrade();
    }

    void OnMouseEnter(){
        _renderer.color = _hoverColor;
        _isUpgrading = true;
    }   
    void OnMouseExit(){
        _renderer.color = _standardColor;
        _isUpgrading = false;
    }

    public bool GetIsUpgrading(){
        return _isUpgrading;
    }
}
