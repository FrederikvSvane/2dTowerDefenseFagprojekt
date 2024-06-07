using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Bullet
{
    [Header("Bomb References")]
    // [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask enemyMask;

    // public Tower parentTower
    [SerializeField] private Enemy enemy;   
    private float _damage;
    private float _range = .5f;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSound;


    public void Start(){
        Physics2D.IgnoreLayerCollision(3, 7);
        _damage = parentTower.GetDamage();
        _audioSource = GetComponent<AudioSource>();
    }

    // public void SetTarget(Transform target){
    //     this.target = target;
    // }
    // Start is called before the first frame update

    // Update is called once per frame
    // private void FixedUpdate()
    // {
    //     if(!target) {
    //         Destroy(gameObject);
    //         return;
    //     };

    //     Vector2 Direction = (target.position - transform.position).normalized;
    //     rb.velocity = Direction * bulletSpeed;   
    // }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        enemy = other.gameObject.GetComponent<Enemy>();

        //Increase Tower Stats
        IncreaseTotalDamage(enemy);
        enemy.TakeDamage(_damage);
        _audioSource.PlayOneShot(_shootSound, .8f);
        //Start explosion damage
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _range, (Vector2) transform.position, 0f, enemyMask);
        foreach(RaycastHit2D hit in hits){
            if(hit.collider.gameObject.tag == "Enemy"){
                enemy = hit.collider.gameObject.GetComponent<Enemy>();
                IncreaseTotalDamage(enemy);
                enemy.TakeDamage(_damage);
                Debug.Log("Hit Enemy from Explosion" + hit.collider.gameObject.name);
            }
        }
        Destroy(gameObject);       
    }

    void IncreaseTotalDamage(Enemy enemy){
        if(enemy.getHealth() >= _damage){
            parentTower.IncreaseDamageDealt(_damage);
        } else {
            parentTower.IncreaseDamageDealt(enemy.getHealth());
        }
    }
}


