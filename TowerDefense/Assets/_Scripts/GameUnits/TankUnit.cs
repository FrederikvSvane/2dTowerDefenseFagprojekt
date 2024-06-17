using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnit : Unit
{
    public TankUnit(float health, float damage, float speed) : base(health, damage, speed)
    {
    }
    public override void Start()
    {
        base.Start();
        SetHealth(400);
        SetSpeed(_moveSpeed * 0.6f);
        transform.localScale = new Vector3(.9f, .9f, 0.5f);
    }
}
