using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D _rb;
    public Tower _parentTower;

    [Header("Bullet Attributes")]
    public Unit _unit;
    private float _bulletSpeed;
    public Transform _target;
    public float _damage;
    public bool _isSlowing;


    public virtual void Start(){
        Physics2D.IgnoreLayerCollision(3, 7);
        _damage = _parentTower.GetDamage();
        SetBulletSpeed(2f);
    }
    public void SetTarget(Transform target){
        this._target = target;
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
        Destroy(gameObject);       
    }

    public bool GetIsSlowing(){
        return _isSlowing;
    }  

    public void SetBulletSpeed(float bulletSpeed){
        _bulletSpeed = bulletSpeed;
    }

    public float GetBulletSpeed(){
        return _bulletSpeed;
    }
}
