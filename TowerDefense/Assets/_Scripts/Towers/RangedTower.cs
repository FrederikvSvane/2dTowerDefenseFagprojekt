using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/*Slowing*/
public class RangedTower : Tower {  
    [Header("Ranged Tower Differentiator")]
    [SerializeField] private bool _isSlowing;
    private int _cost;

    public RangedTower(){
        InitializeTower();
    }
    

    protected override void Start()
    {
        base.Start();
        InitializeTower();
    }
    private void InitializeTower(){
        if(_isSlowing){
            _damage = 10; // per attack
            _cost = 200;
        } else {
            _damage = 20; // per attack
            _cost = 200;
        }
        _range = 5; // in tiles
        _bulletReloadSpeed = 2f;
    }

    public override Unit ClosestToEndUnit(RaycastHit2D[] hits)
    {
        return base.ClosestToEndUnit(hits);
    }

    public override float GetCost(){
        return _cost;
    }

    public override Tower BuyTower(Player player, Transform transform)
    {
        InitializeTower();
        string towerType = this.GetType().ToString();
        GameObject towerPrefab = _towerManager.GetTowerPrefab(towerType);
        GameObject tower = PhotonNetwork.Instantiate(towerPrefab.name, transform.position, Quaternion.identity);
        player.SubtractCoinsFromBalance(_cost);
        return tower.GetComponent<Tower>(); // This MIGHT work
    }
}