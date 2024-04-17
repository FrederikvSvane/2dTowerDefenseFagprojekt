using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTower : MonoBehaviour
{
 public Player playerRef;
 public Tower newTower;
 public Button myButton;

 public void ChangeTower(){
    playerRef.setTower(newTower);
 }

 public void Start(){
    myButton.onClick.AddListener(ChangeTower);
    
 }
}
