using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player Resources")]
    [SerializeField] private float health;
    [SerializeField] private float coins;
    [SerializeField] private String playerName;


    
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buyTower(float amount){
        coins -= amount;
    }

    public float getCoinBalance(){
        return coins;
    }

    public void takeDamage(float amount){
        health -= amount;
    }
}
