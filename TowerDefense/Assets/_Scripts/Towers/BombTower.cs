using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombTower : Tower
{
    // Start is called before the first frame update
    private int _cost;
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
        _damage = 30; // per attack
        _range = 5; // in tiles
        _cost = 400; //in gold
        _bulletReloadSpeed = .5f;
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

    public override float GetCost(){
        return _cost;
    }

}
