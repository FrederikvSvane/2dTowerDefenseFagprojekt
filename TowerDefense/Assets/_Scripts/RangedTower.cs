using System.Collections;
using System.Collections.Generic;
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
        damage = 30; // per attack
        range = 5; // in tiles
        firingRate = 1f;
        cost = 100; //in gold
        bulletReloadSpeed = 2f;

    }

    public override float getCost(){
        return base.cost;
    }

    public override Tower buyTower(Player player, Transform transform)
    {
        InitializeTower();
        Tower tower = Instantiate(this, transform.position, Quaternion.identity);
        player.buyTower(cost);
        Debug.Log("Tower bought for " + cost + " gold");
        return tower;
    }





}