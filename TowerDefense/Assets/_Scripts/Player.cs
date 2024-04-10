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

    public void SetHealth(float amount){
        this.health = amount;
    }
    public void SetCoinBalance(float amount){
        this.coins = amount;
    }

    public void SubtractCoinsFromBalance(float amount){
        coins -= amount;
    }

    public float GetCoinBalance(){
        return this.coins;
    }

    public void SubtractHealthFromBalance(float amount){
        health -= amount;
    }
}
