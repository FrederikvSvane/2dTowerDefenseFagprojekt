using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBullets : Bullet
{
    public SlowBullets(){
        InitializeBullet();
    }

    public override void Start()
    {
        base.Start();
        InitializeBullet();
        _isSlowing = true;
    }

    void InitializeBullet(){
        _bulletSpeed = 2f;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(!_target) {
            Destroy(gameObject);
            return;
        };

        Vector2 Direction = (_target.position - transform.position).normalized;
        _rb.velocity = Direction * _bulletSpeed;   
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        _unit = other.gameObject.GetComponent<Unit>();
        if(_unit == null) return;
        if(_unit.GetHealth() >= _damage){
            _parentTower.IncreaseDamageDealt(_damage);
        } else {
            _parentTower.IncreaseDamageDealt(_unit.GetHealth());
        }
        _unit.TakeDamage(_damage);
        _unit.Slow();
        Destroy(gameObject);               
    }
}
