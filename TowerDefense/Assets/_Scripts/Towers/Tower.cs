using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private LayerMask unitMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;

    protected TowerManager _towerManager;

    [Header("Tower Attributes")]    
    public float health;
    public float damage;
    public float range;
    public float cost;
    public float rotSpeed = 25f;
    public Transform unitTarget;
    public float bulletReloadSpeed;
    public float firingRate;
    private string towerPrefab;

    // Enum for target types
    public enum TargetType
    {
        MostHealth,
        LeastHealth,
        FurthestFromEnd,
        ClosestToEnd
    }

    [Header("Stats")]
    [SerializeField] private float totalDamage;

    private float timeBetweenTargetUpdate = 0.5f;

    private float time = 0f;

    //Brug raycast istedet ;)
    // Start is called before the first frame update
    protected virtual void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        _towerManager = FindObjectOfType<TowerManager>();
    }

    // Update is called once per frame
    public virtual void Update()
    {   

        if(unitTarget == null){
            TargetUnit();
        }        
        time += Time.deltaTime;
        if(time >= timeBetweenTargetUpdate){
            TargetUnit();
            time = 0f;
        }

        RotateTower();

        if(!CheckTargetInRange()){
            unitTarget = null;
        } else {
            firingRate += Time.deltaTime;
            if(firingRate >= 1/bulletReloadSpeed){
                Attack();
                firingRate = 0f;
            }
        };
    }

    private bool CheckTargetInRange(){
        if (unitTarget != null)
        {
            return Vector2.Distance(transform.position, unitTarget.position) <= range;
        }
        return false;
    }

    private void TargetUnit(){
        //Circular raycast, from tower position, with range also it only hits units that are on the unit layermask.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2) transform.position, 0f, unitMask);
        if(hits.Length > 0){
            //unitTarget = hits[0].transform;
            //Target the unit closest to the end
            unitTarget = ClosestToEndUnit(hits).transform;
            


        }

    }

    private RaycastHit2D LowestHealthUnit(RaycastHit2D[] hits){
        RaycastHit2D lowestHealthUnit = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Unit>().getHealth() < lowestHealthUnit.transform.GetComponent<Unit>().getHealth()){
                lowestHealthUnit = hit;
            }
        }
        return lowestHealthUnit;
    }


    private RaycastHit2D MostHealthUnit(RaycastHit2D[] hits){
        RaycastHit2D mostHealthUnit = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Unit>().getHealth() > mostHealthUnit.transform.GetComponent<Unit>().getHealth()){
                mostHealthUnit = hit;
            }
        }
        return mostHealthUnit;
    }

    private Unit ClosestToEndUnit(RaycastHit2D[] hits){
        Unit closestToEndUnit = hits[0].transform.GetComponent<Unit>();
        foreach(RaycastHit2D hit in hits){
            Unit hitUnit = hit.transform.GetComponent<Unit>();
            bool isMine = hitUnit._photonView.IsMine;
            if(hitUnit.GetDistanceFromEnd() < closestToEndUnit.GetDistanceFromEnd() && isMine){
                closestToEndUnit = hit.transform.GetComponent<Unit>();
            }
        }
        return closestToEndUnit;
    }

    private RaycastHit2D FurthestFromEndUnit(RaycastHit2D[] hits){
        RaycastHit2D furthestFromEndUnit = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Unit>().GetDistanceFromEnd() > furthestFromEndUnit.transform.GetComponent<Unit>().GetDistanceFromEnd()){
                furthestFromEndUnit = hit;
            }
        }
        return furthestFromEndUnit;
    }

    

    private void RotateTower(){
        float angle = 0f;
        float idleRotAngle = 0f;

        if (unitTarget != null)
        {
            angle = Mathf.Atan2(unitTarget.position.y - transform.position.y, unitTarget.position.x - transform.position.x) * Mathf.Rad2Deg;

            //Quarternion.Euler is used to convert the angle to a rotation
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, targetRotation, rotSpeed * Time.deltaTime);
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
            rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, Quaternion.Euler(new Vector3(0, 0, idleRotAngle)), rotSpeed / 2 * Time.deltaTime);
        }

    }
    public virtual void Attack()
    {
        //attack the unit
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
    
        audioSource.PlayOneShot(shootSound, .3f);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.parentTower = this;
        bulletScript.SetTarget(unitTarget);
    }

    public void setPrefab(string prefabName){
        towerPrefab = prefabName;
    }

    public string getPrefab(){
        return towerPrefab;
    }
    public void IncreaseDamageDealt(float damage){
        totalDamage += damage;
    }

    

    public float GetRange(){
        return range;
    }

    public float GetDamage(){
        return damage;
    }

    public float GetCost(){
        return cost;
    }

    public void Suicide(){
        Destroy(this.gameObject);
    }
    public virtual float getCost(){return cost;}
    
    public abstract Tower buyTower(Player player, Transform transform);
}