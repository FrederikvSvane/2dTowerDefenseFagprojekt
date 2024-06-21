using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Player Resources")]
    [SerializeField] private float health;
    [SerializeField] private float coins;
    public string NickName { get; private set; }

    public TextMeshProUGUI healthText, coinText;
    //public GameObject gameOver;
    public Tower _activeTower;
    public Tower _selectedTower;
    
    // Update is called once per frame
    void Update()
    {
        healthText.text = "" + health;
        coinText.text = "" + (int) coins;

        if (health <= 0){
            //GameOver();
        }
    }

    public void SetHealth(float amount){
        health = amount;
    }
    public void SetCoinBalance(float amount){
        coins = amount;
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

    public void GameOver(){
        //gameOver.SetActive(true);
    }
    public Tower getTower(){
        return _activeTower;
    }
    public void SetTower(Tower tower){
        _activeTower = tower;
    }

    public void SetSelectedTower(Tower tower){
        _selectedTower = tower;
    }   

    public Tower GetSelectedTower(){
        return _selectedTower;
    }
}
