using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    public Tower parentTower;

    [Header("Bullet Attributes")]
    [SerializeField] private float bulletSpeed = 0.5f;

    private Transform target;
    [SerializeField] private Enemy enemy;
    private float damage;


    public void Start(){
        Physics2D.IgnoreLayerCollision(3, 7);
        damage = parentTower.GetDamage();

    }
    public void SetTarget(Transform target){
        this.target = target;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(!target) {
            Destroy(gameObject);
            return;
        };

        Vector2 Direction = (target.position - transform.position).normalized;
        rb.velocity = Direction * bulletSpeed;   
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        enemy = other.gameObject.GetComponent<Enemy>();
        //TODO: Deal damage
        //Debug.Log("Hit Enemy " + other.gameObject.name);
        if(enemy.getHealth() >= damage){
            parentTower.IncreaseDamageDealt(damage);
        } else {
            parentTower.IncreaseDamageDealt(enemy.getHealth());
        }
        enemy.TakeDamage(damage);
        Destroy(gameObject);       
    }
}
