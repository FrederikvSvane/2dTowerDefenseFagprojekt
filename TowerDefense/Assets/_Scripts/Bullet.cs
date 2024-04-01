using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Bullet Attributes")]
    [SerializeField] private float bulletSpeed = 5f;

    private Transform target;

    public void SetTarget(Transform target){
        this.target = target;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(!target) return;

        Vector2 Direction = (target.position - transform.position).normalized;
        rb.velocity = Direction * bulletSpeed;   
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
