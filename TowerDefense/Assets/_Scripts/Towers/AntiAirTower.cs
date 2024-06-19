using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AntiAirTower : Tower
{
    // Start is called before the first frame update
    private int _cost;
    public AntiAirTower(){
        InitializeTower();
    }
    

    protected override void Start()
    {
        base.Start();
        InitializeTower();
    }
    private void InitializeTower(){
        _damage = 20; // per attack
        _bulletReloadSpeed = 5f; //Higher is faster.
        _cost = 500;
    }

    public override Unit ClosestToEndUnit(RaycastHit2D[] hits)
    {
        List<Unit> units = new List<Unit>();
        foreach (RaycastHit2D hit in hits)
        {
            //Debug.Log(hit.transform.name);
            Unit unit = hit.transform.GetComponent<Unit>();
            bool isSameOwner = unit._photonView.Owner.UserId == GetPhotonView().Owner.UserId;
            if (isSameOwner && unit.GetIsFlying())
                units.Add(hit.transform.GetComponent<Unit>());
        }

        if (units.Count == 0)
            return null;

        Unit closestToEndUnit = units[0];
        foreach (RaycastHit2D hit in hits)
        {
            Unit hitUnit = hit.transform.GetComponent<Unit>();
            bool isSameOwner = hitUnit._photonView.Owner.UserId == GetPhotonView().Owner.UserId;
            if (hitUnit.GetDistanceFromEnd() < closestToEndUnit.GetDistanceFromEnd() && isSameOwner)
            {
                closestToEndUnit = hit.transform.GetComponent<Unit>();
            }
        }
        return closestToEndUnit;
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

