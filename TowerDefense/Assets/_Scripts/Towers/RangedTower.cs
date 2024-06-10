using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RangedTower : Tower{

    public RangedTower(){
        InitializeTower();
    }
    

    protected override void Start()
    {
        base.Start();
        InitializeTower();
    }
    private void InitializeTower(){
        health = 100; // in hitpoints
        damage = 20; // per attack
        range = 5; // in tiles
        firingRate = 5f;
        cost = 250; //in gold
        bulletReloadSpeed = 2f;

    }

    public override float getCost(){
        return base.cost;
    }

    public override Tower buyTower(Player player, Transform transform)
    {
        InitializeTower();
        string towerType = this.GetType().ToString();
        GameObject towerPrefab = _towerManager.GetTowerPrefab(towerType);
        GameObject tower = PhotonNetwork.Instantiate(towerPrefab.name, transform.position, Quaternion.identity);
        player.SubtractCoinsFromBalance(cost);
        return tower.GetComponent<Tower>(); // This MIGHT work
    }





}