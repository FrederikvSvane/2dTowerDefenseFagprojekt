using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : Unit
{

    public FlyingUnit() 
    {
        _costToSend = 100f;
        _incomeIncrease = 20f;
    }
    public override void Start()
    {
        base.Start();
        SetHealth(120);
        SetSpeed(_moveSpeed * 1.2f);

        _isFlying = true;
    }
    public override float GetCostToSend()
    {
        return _costToSend;
    }
}
