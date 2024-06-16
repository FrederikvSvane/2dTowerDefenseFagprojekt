using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiAirBullet : Bullet
{
    public AntiAirBullet(){
        InitializeBullet();
    }

    public override void Start()
    {
        base.Start();
        InitializeBullet();
    }

    public void InitializeBullet(){
        damage = 20;
        bulletSpeed = 0.8f;
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
        unit = other.gameObject.GetComponent<Unit>();
        if(unit == null) return;
        if(unit.GetIsFlying()){
        if(unit.getHealth() >= damage){
            parentTower.IncreaseDamageDealt(damage);
        } else {
            parentTower.IncreaseDamageDealt(unit.getHealth());
        }
        unit.TakeDamage(damage);
        Destroy(gameObject);  
        }
             
    }
}
