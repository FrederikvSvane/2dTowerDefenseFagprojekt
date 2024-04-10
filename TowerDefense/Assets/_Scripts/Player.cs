using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player Resources")]
    [SerializeField] private float health;
    [SerializeField] private float coins;
    [SerializeField] private String playerName = "Hej";

    public TextMeshProUGUI healthText, coinText, playerNameText;
    


    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + health;
        coinText.text = "Coins: " + coins;
        playerNameText.text =playerName;
    }

    public void setHealth(float amount){
        this.health = amount;
    }
    public void setCoinBalance(float amount){
        this.coins = amount;
    }

    public void buyTower(float amount){
        coins -= amount;
    }

    public float getCoinBalance(){
        return this.coins;
    }

    public void takeDamage(float amount){
        health -= amount;
    }
}
