using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rotationPoint;
    [SerializeField] private LayerMask enemyMask;

    [Header("Tower Attributes")]
    [SerializeField] public float health;
    [SerializeField] public float damage;
    [SerializeField] public float range;
    [SerializeField] public float attackSpeed;
    [SerializeField] public float cost;
    [SerializeField] public float rotSpeed = 25f;

    public Transform enemyTarget;

    //Brug raycast istedet ;)
    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(enemyTarget == null){
            TargetEnemy();
            return;
        } 

        RotateTower();

        if(!CheckTargetInRange()){
            enemyTarget = null;
        };
    }

    private bool CheckTargetInRange(){
        return Vector2.Distance(transform.position, enemyTarget.position) <= range;
    }

    private void TargetEnemy(){
        //Circular raycast, from tower position, with range also it only hits enemies that are on the enemy layermask.
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, range, (Vector2) transform.position, 0f, enemyMask);
        if(hits.Length > 0){
            enemyTarget = hits[0].transform;
        }
    }

    private void RotateTower(){
        float angle = Mathf.Atan2(enemyTarget.position.y - transform.position.y, enemyTarget.position.x - transform.position.x) * Mathf.Rad2Deg;
        float idleRotAngle = 0f;

        //Quarternion.Euler is used to convert the angle to a rotation
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, targetRotation, rotSpeed * Time.deltaTime);

        if(enemyTarget == null){
            if(rotationPoint.rotation.z == 75f){
                idleRotAngle = 75f;
            } else if(rotationPoint.rotation.z == -75f){
                idleRotAngle = -75f;
            }
        rotationPoint.rotation = Quaternion.RotateTowards(rotationPoint.rotation, Quaternion.Euler(new Vector3(0, 0, idleRotAngle)), rotSpeed / 2 * Time.deltaTime);
        }

    }
    public virtual void Attack()
    {
        //attack the enemy
    }

    

    public float getRange(){
        return range;
    }

}
