using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;

public class TowerInformation : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private TextMeshProUGUI _damage;
    [SerializeField] private TextMeshProUGUI _attackSpeed;
    [SerializeField] private TextMeshProUGUI _range;
    [SerializeField] private TextMeshProUGUI _totalDamage;
    [SerializeField] private Tower _tower;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {   
        _tower = _player.GetSelectedTower();
        if(_tower != null){
            _damage.text =  _tower.GetDamage().ToString();
            _attackSpeed.text =  _tower.GetBulletReloadSpeed().ToString();
            _range.text = _tower.GetRange().ToString();
            _totalDamage.text = _tower.GetTotalDamage().ToString();
        }
    }
}
