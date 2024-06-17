using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnit : Unit
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        SetSpeed(_moveSpeed * 0.6f);
        _isFlying = true;
    }
}
