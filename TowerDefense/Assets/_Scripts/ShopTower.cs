using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTower : MonoBehaviour
{
 public Player playerRef;
 public Tower newTower;
 public Button myButton;

 
 public void ChangeToRedTower(){
   newTower.setPrefab("RangedRedTower");
   playerRef.setTower(newTower);
 }

public void ChangeToWhiteTower(){
      newTower.setPrefab("RangedWhiteTower");
      playerRef.setTower(newTower);
}

public void ChangeToBombTower(){
      newTower.setPrefab("BombTower");
      playerRef.setTower(newTower);
}

 public void Start(){
    //myButton.onClick.AddListener(ChangeTower);
    
 }
}
