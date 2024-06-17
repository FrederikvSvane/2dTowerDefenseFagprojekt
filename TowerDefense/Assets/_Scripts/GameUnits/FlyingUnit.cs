using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : Unit
{
    public FlyingUnit(float health, float damage, float speed) : base(health, damage, speed)
    {
    }
    public override void Start()
    {
        base.Start();
        SetSpeed(_moveSpeed * 0.6f);
        _isFlying = true;
    }
}
