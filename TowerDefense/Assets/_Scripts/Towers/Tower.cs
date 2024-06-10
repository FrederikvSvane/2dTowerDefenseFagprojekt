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
    [SerializeField] private LayerMask enemyMask;
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
    public Transform enemyTarget;
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

        if(enemyTarget == null){
            TargetEnemy();
        }        
        time += Time.deltaTime;
        if(time >= timeBetweenTargetUpdate){
            TargetEnemy();
            time = 0f;
        }

        RotateTower();

        if(!CheckTargetInRange()){
            enemyTarget = null;
        } else {
            firingRate += Time.deltaTime;
            if(firingRate >= 1/bulletReloadSpeed){
                Attack();
                firingRate = 0f;
            }
        };
    }

    private bool CheckTargetInRange(){
        if (enemyTarget != null)
        {
            return Vector2.Distance(transform.position, enemyTarget.position) <= range;
        }
        return false;
    }

    private void TargetEnemy(){
        //Circular raycast, from tower position, with range also it only hits enemies that are on the enemy layermask.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2) transform.position, 0f, enemyMask);
        if(hits.Length > 0){
            //enemyTarget = hits[0].transform;
            //Target the enemy closest to the end
            enemyTarget = ClosestToEndEnemy(hits).transform;
            


        }

    }

    //Find the enemy with the lowest health
    private RaycastHit2D LowestHealthEnemy(RaycastHit2D[] hits){
        RaycastHit2D lowestHealthEnemy = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Enemy>().getHealth() < lowestHealthEnemy.transform.GetComponent<Enemy>().getHealth()){
                lowestHealthEnemy = hit;
            }
        }
        return lowestHealthEnemy;
    }


    //Find the enemy with the most health
    private RaycastHit2D MostHealthEnemy(RaycastHit2D[] hits){
        RaycastHit2D mostHealthEnemy = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Enemy>().getHealth() > mostHealthEnemy.transform.GetComponent<Enemy>().getHealth()){
                mostHealthEnemy = hit;
            }
        }
        return mostHealthEnemy;
    }

    //Find the enemy closest to the end
    private RaycastHit2D ClosestToEndEnemy(RaycastHit2D[] hits){
        RaycastHit2D closestToEndEnemy = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Enemy>().getDistanceFromEnd() < closestToEndEnemy.transform.GetComponent<Enemy>().getDistanceFromEnd()){
                closestToEndEnemy = hit;
            }
        }
        return closestToEndEnemy;
    }

    //Find the enemy furthest from the end
    private RaycastHit2D FurthestFromEndEnemy(RaycastHit2D[] hits){
        RaycastHit2D furthestFromEndEnemy = hits[0];
        foreach(RaycastHit2D hit in hits){
            if(hit.transform.GetComponent<Enemy>().getDistanceFromEnd() > furthestFromEndEnemy.transform.GetComponent<Enemy>().getDistanceFromEnd()){
                furthestFromEndEnemy = hit;
            }
        }
        return furthestFromEndEnemy;
    }

    

    private void RotateTower(){
        float angle = 0f;
        float idleRotAngle = 0f;

        if (enemyTarget != null)
        {
            angle = Mathf.Atan2(enemyTarget.position.y - transform.position.y, enemyTarget.position.x - transform.position.x) * Mathf.Rad2Deg;

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
        //attack the enemy
        //Debug.Log("Attacking Enemy");
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
    
        audioSource.PlayOneShot(shootSound, .3f);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.parentTower = this;
        bulletScript.SetTarget(enemyTarget);
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