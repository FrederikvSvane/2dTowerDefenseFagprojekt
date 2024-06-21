using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Bullet
{
    [Header("Bomb References")]
    // [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask unitMask;

    // public Tower parentTower
    [SerializeField] private Unit unit;
    private float _range = 2f;
    private float _blastDamage = 10f;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSound;


    public Bomb(){
        InitializeBullet();
    }

    public override void Start()
    {
        base.Start();
        InitializeBullet();
        _audioSource = GetComponent<AudioSource>();
    }

    public void InitializeBullet(){
        SetBulletSpeed(5f);
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        unit = other.gameObject.GetComponent<Unit>();

        //Increase Tower Stats
        unit.TakeDamage(_damage);
        IncreaseTotalDamage(unit);
        _audioSource.PlayOneShot(_shootSound, .8f);
        //Start explosion damage
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _range, (Vector2)transform.position, 0f, unitMask);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "Unit")
            {
                unit = hit.collider.gameObject.GetComponent<Unit>();
                IncreaseTotalDamage(unit);
                unit.TakeDamage(_blastDamage);
            }
        }
        Destroy(gameObject);
    }

    void IncreaseTotalDamage(Unit unit)
    {
        if (unit != null && unit.GetHealth() >= _damage)
        {
            _parentTower.IncreaseDamageDealt(_damage);
        }
        else if (unit != null && unit.GetHealth() < _damage)
        {
            _parentTower.IncreaseDamageDealt(unit.GetHealth());
        }
    }
}


