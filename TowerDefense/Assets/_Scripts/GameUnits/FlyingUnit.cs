using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : Unit
{
    public FlyingUnit() 
    {
    }
    public override void Start()
    {
        base.Start();
        SetHealth(120);
        SetSpeed(_moveSpeed * 0.6f);
        _isFlying = true;
    }
}
