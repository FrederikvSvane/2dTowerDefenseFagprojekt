using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float health;
    public float damage;
    public float range;
    public float attackSpeed;
    public float cost;
    private CircleCollider2D rangeCollider;

    //Brug raycast istedet ;)
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rangeCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        rangeCollider.radius = range;
    }

    public virtual void Attack()
    {
        //attack the enemy
    }

    private void OnTriggerEnter2D(Collider coll)
    {
        Debug.Log("Tower has detected an enemy");

    } 

    public float getRange(){
        return range;
    }

}
