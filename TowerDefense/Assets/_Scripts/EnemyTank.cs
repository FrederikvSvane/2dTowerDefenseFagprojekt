using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : Enemy
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        setHealth(400);
        setSpeed(moveSpeed * 0.4f);
        transform.localScale = new Vector3(1.5f, 1.5f, 0.5f);
    }

}
