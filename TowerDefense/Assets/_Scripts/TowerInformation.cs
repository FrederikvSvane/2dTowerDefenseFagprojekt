using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;

public class TowerInformation : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private TextMeshProUGUI _towerName, _damage, _attackSpeed, _range, _totalDamage, _upgradeSellCost;
    [SerializeField] private Tower _tower;

    // Update is called once per frame
    void Update()
    {   
        _tower = _player.GetSelectedTower();
        if (_tower == null) return;

        bool isUpgrading = _tower.GetUpgrading();
        if(_tower != null && !isUpgrading){
            _upgradeSellCost.text = "+ " + (_tower.GetCost() * 0.7).ToString();
            _towerName.text = _tower.GetType().ToString() + " Level: " + _tower.GetLevel().ToString();
            _damage.text =  _tower.GetDamage().ToString();
            _attackSpeed.text =  _tower.GetBulletReloadSpeed().ToString();
            _range.text = _tower.GetRange().ToString();
            _totalDamage.text = _tower.GetTotalDamage().ToString();
        } else if (_tower != null && isUpgrading){
            _upgradeSellCost.text = "- " + _tower.CalculateUpgradeCost(0.2f).ToString();
            _towerName.text = _tower.GetType().ToString() + " Level: " + _tower.GetLevel().ToString() + " -> " + (_tower.GetLevel() + 1).ToString();
            _damage.text =  _tower.GetDamage().ToString() + " + " + _tower.GetUpgradedDamage().ToString();
            _attackSpeed.text =  _tower.GetBulletReloadSpeed().ToString() + " + " + _tower.GetUpgradedBulletReloadSpeed().ToString();
            _range.text = _tower.GetRange().ToString() + " + " + _tower.GetUpgradedRange().ToString();
            _totalDamage.text = _tower.GetTotalDamage().ToString();
        }
    }
}
