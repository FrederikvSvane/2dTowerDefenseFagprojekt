using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTypes : MonoBehaviour
{
    public float _health;
    public float _moveSpeed;
    public float _damage;
public UnitTypes(float health, float speed, float damage)
    {
        this._health = health;
        this._moveSpeed = speed;
        this._damage = damage;
    }

}
