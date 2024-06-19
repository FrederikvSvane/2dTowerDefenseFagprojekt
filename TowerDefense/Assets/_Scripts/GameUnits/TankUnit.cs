using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnit : Unit
{
    public TankUnit() 
    {
    }
    public override void Start()
    {
        base.Start();
        SetHealth(400);
        SetSpeed(_moveSpeed * 0.6f);
    }
}
