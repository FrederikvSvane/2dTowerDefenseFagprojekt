using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombTower : Tower
{

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombSpawnPoint;
    // Start is called before the first frame update
    public BombTower(){
        InitializeTower();
    }

    protected override void Start()
    {
        base.Start();
        InitializeTower();
    }

    // Update is called once per frame
    private void InitializeTower(){
        health = 100; // in hitpoints
        damage = 30; // per attack
        range = 5; // in tiles
        cost = 100; //in gold
        bulletReloadSpeed = .5f;
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

    // public override void Attack()
    // {
    //     base.Attack();
    //     GameObject bomb = Instantiate(bombPrefab, bombSpawnPoint.position, Quaternion.identity);
    //     //attack the enemy
    //     //Debug.Log("Attacking Enemy");
    //     audioSource.PlayOneShot(shootSound, .3f);
    //     Bomb bombScript = bomb.GetComponent<Bomb>();
    //     bombScript.parentTower = this;
    //     bombScript.SetTarget(enemyTarget);
    // }
}
