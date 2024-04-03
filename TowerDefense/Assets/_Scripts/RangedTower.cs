using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedTower : Tower{

    [Header("Ranged Tower Attributes")]
    public float damagePerSecond; 

    protected override void Start()
    {
        base.Start();
        health = 100; // in hitpoints
        damage = 30; // per attack
        range = 5; // in tiles
        attackSpeed = 1; //attacks per second
        cost = 100; //in gold
        damagePerSecond = damage * attackSpeed;
        bulletReloadSpeed = 2f;
    }

    public override void Update()
    {
        base.Update();
    }


    
}