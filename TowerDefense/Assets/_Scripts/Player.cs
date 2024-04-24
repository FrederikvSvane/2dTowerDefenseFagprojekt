using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player Resources")]
    [SerializeField] private float health;
    [SerializeField] private float coins;
    [SerializeField] private String playerName;
    // private GUID playerId;

    public TextMeshProUGUI healthText, coinText, playerNameText;
    public GameObject gameOver;
    public Tower _activeTower;
    
    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + health;
        coinText.text = "Coins: " + coins;
        playerNameText.text = playerName;

        if (health <= 0){
            //GameOver();
        }
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

    public void AddCoinsToBalance(float amount){
        coins += amount;
    }

    public float GetCoinBalance(){
        return this.coins;
    }

    public void SubtractHealthFromBalance(float amount){
        health -= amount;
    }

    public void getCoinFromEnemyKill(Enemy enemy){
        coins += enemy.getOnKillValue();
    }

    public void GameOver(){
        gameOver.SetActive(true);
    }
    public Tower getTower(){
        return _activeTower;
    }
    public void setTower(Tower tower){
        _activeTower = tower;
    }
}
