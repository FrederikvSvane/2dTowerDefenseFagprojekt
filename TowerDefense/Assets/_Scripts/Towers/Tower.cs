using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
public abstract class Tower : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField] private Transform rotationPoint;
    public LayerMask unitMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    protected TowerManager _towerManager;
    private PhotonView _photonView;
    [SerializeField] private GameObject _sellOrUpgradeMenu;
    [SerializeField] private GameObject _upgradeButton;
    [SerializeField] private GameObject _sellButton;

    [Header("Tower Attributes")]
    public float _damage;
    public float _range;
    private float _cost;
    private float _rotSpeed = 100f;
    public Transform _unitTarget;
    public float _bulletReloadSpeed;
    private float _firingRate;
    private string _towerPrefab;
    private Tile _tile;
    private int _level = 1;
    
    private Player _player;

    // Enum for target types
    public enum TargetType
    {
        MostHealth,
        LeastHealth,
        FurthestFromEnd,
        ClosestToEnd
    }

    [Header("Stats")]
    private float _totalDamage;
    private float _timeBetweenTargetUpdate = 0.5f;
    private float _time = 0f;

    //Brug raycast istedet ;)
    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        _towerManager = FindObjectOfType<TowerManager>();
        _photonView = GetComponent<PhotonView>();
        _player = FindObjectOfType<Player>();
        _range = 10f;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (_unitTarget == null)
        {
            Debug.Log("Tower Update: " + _unitTarget);
            TargetUnit();
        }
        _time += Time.deltaTime;
        if (_time >= _timeBetweenTargetUpdate)
        {
            TargetUnit();
            _time = 0f;
        }

        RotateTower();

        if (CheckTargetInRange())
        {
            _firingRate += Time.deltaTime;
            if (_firingRate >= 1 / _bulletReloadSpeed)
            {
                Attack();
                _firingRate = 0f;
            }
        }
    }

    private bool CheckTargetInRange()
    {
        if (_unitTarget != null)
        {
            return Vector2.Distance(transform.position, _unitTarget.position) <= _range;
        }
        return false;
    }

    private void TargetUnit()
    {
        //Circular raycast, from tower position, with range also it only hits units that are on the unit layermask.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _range, (Vector2)transform.position, 0f, unitMask);
        if (hits.Length > 0)
        {
            Debug.Log("Target Acquired");
            //unitTarget = hits[0].transform;
            //Target the unit closest to the end
            _unitTarget = ClosestToEndUnit(hits) ? ClosestToEndUnit(hits).transform : null;
        }
    }

    private RaycastHit2D LowestHealthUnit(RaycastHit2D[] hits)
    {
        RaycastHit2D lowestHealthUnit = hits[0];
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Unit>().GetHealth() < lowestHealthUnit.transform.GetComponent<Unit>().GetHealth())
            {
                lowestHealthUnit = hit;
            }
        }
        return lowestHealthUnit;
    }


    private RaycastHit2D MostHealthUnit(RaycastHit2D[] hits)
    {
        RaycastHit2D mostHealthUnit = hits[0];
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Unit>().GetHealth() > mostHealthUnit.transform.GetComponent<Unit>().GetHealth())
            {
                mostHealthUnit = hit;
            }
        }
        return mostHealthUnit;
    }

    public virtual Unit ClosestToEndUnit(RaycastHit2D[] hits)
    {
        List<Unit> units = new List<Unit>();
        foreach (RaycastHit2D hit in hits)
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            bool isSameOwner = unit._photonView.Owner.UserId == _photonView.Owner.UserId;
            if (isSameOwner)
                units.Add(hit.transform.GetComponent<Unit>());
        }

        if (units.Count == 0)
            return null;

        Unit closestToEndUnit = units[0];
        foreach (RaycastHit2D hit in hits)
        {
            Unit hitUnit = hit.transform.GetComponent<Unit>();
            bool isSameOwner = hitUnit._photonView.Owner.UserId == _photonView.Owner.UserId;
            if (hitUnit.GetDistanceFromEnd() < closestToEndUnit.GetDistanceFromEnd() && isSameOwner)
            {
                closestToEndUnit = hit.transform.GetComponent<Unit>();
            }
        }
        return closestToEndUnit;
    }

    private RaycastHit2D FurthestFromEndUnit(RaycastHit2D[] hits)
    {
        RaycastHit2D furthestFromEndUnit = hits[0];
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.GetComponent<Unit>().GetDistanceFromEnd() > furthestFromEndUnit.transform.GetComponent<Unit>().GetDistanceFromEnd())
            {
                furthestFromEndUnit = hit;
            }
        }
        return furthestFromEndUnit;
    }



    private void RotateTower()
    {
        float angle = 0f;
        float idleRotAngle = 0f;

        if (_unitTarget != null)
        {
            angle = Mathf.Atan2(_unitTarget.position.y - transform.position.y, _unitTarget.position.x - transform.position.x) * Mathf.Rad2Deg;

            //Quarternion.Euler is used to convert the angle to a rotation
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, targetRotation, _rotSpeed * Time.deltaTime);
        }
        else
        {
            if (rotationPoint.rotation.z == 75f)
            {
                idleRotAngle = 75f;
            }
            else if (rotationPoint.rotation.z == -75f)
            {
                idleRotAngle = -75f;
            }
            rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, Quaternion.Euler(new Vector3(0, 0, idleRotAngle)), _rotSpeed / 2 * Time.deltaTime);
        }

    }
    public virtual void Attack()
    {
        //attack the unit
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        audioSource.PlayOneShot(shootSound, .3f);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript._parentTower = this;
        bulletScript.SetTarget(_unitTarget);
    }

    public void TriggerSell(){
        _tile.SellTower(0.7f);
        
    }

    public void TriggerUpgrade(){
        int upgradeCost = CalculateUpgradeCost(0.2f);
        if (_player.GetCoinBalance() >= upgradeCost && _level < 100){
            _damage += GetUpgradedDamage();
            _range += GetUpgradedRange();
            _bulletReloadSpeed += GetUpgradedBulletReloadSpeed();
            _level++;
            _player.SubtractCoinsFromBalance(upgradeCost);
            IncreaseCostAfterUpgrade(upgradeCost);
        }        
    }

    public int CalculateUpgradeCost(float growthFactor){
        return (int) GetCost() * (int) Math.Pow(1 + growthFactor, _level - 1);
    }

    public float GetUpgradedRange() { return 0.5f; }
    public int GetUpgradedDamage() { return 5; }
    public float GetUpgradedBulletReloadSpeed() { return 0.1f; } 
    public int GetLevel() { return _level; }

    public void SetPrefab(string prefabName)
    {
        _towerPrefab = prefabName;
    }

    public string getPrefab()
    {
        return _towerPrefab;
    }
    public void IncreaseDamageDealt(float damage)
    {
        _totalDamage += damage;
    }

    public float GetRange()
    {
        return _range;
    }

    public float GetDamage()
    {
        return _damage;
    }

    public float GetTotalDamage()
    {
        return _totalDamage;
    }

    public float GetBulletReloadSpeed()
    {
        return _bulletReloadSpeed;
    }

    public Tile GetTile(){
        return _tile;
    }

    public void SetTile(Tile tile){
        _tile = tile;
    }

    public void ToggleSellOrUpgradeMenu(bool enable){
        if (_sellOrUpgradeMenu != null)
            _sellOrUpgradeMenu.SetActive(enable);
    }

    public bool GetUpgrading(){
        return _upgradeButton.GetComponent<Upgrade>().GetIsUpgrading();
    }

    public void Suicide()
    {
        Destroy(this.gameObject);
    }

    public void IncreaseCostAfterUpgrade(float cost){
        this._cost += cost;
    }

    public PhotonView GetPhotonView(){
        return _photonView;
    }
    public virtual float GetCost() { return _cost; }

    public abstract Tower BuyTower(Player player, Transform transform);
}