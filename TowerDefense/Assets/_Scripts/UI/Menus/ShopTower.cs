using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTower : MonoBehaviour
{
  public Player playerRef;
  public Tower newTower;
  public Button myButton;


  public void ChangeToRedTower()
  {
    newTower.SetPrefab("RangedRedTower");
    playerRef.SetTower(newTower);
  }

  public void ChangeToWhiteTower()
  {
    newTower.SetPrefab("RangedWhiteTower");
    playerRef.SetTower(newTower);
  }

  public void ChangeToBombTower()
  {
    newTower.SetPrefab("BombTower");
    playerRef.SetTower(newTower);
  }

  public void ChangeToAntiAirTower(){
        newTower.SetPrefab("AntiAirTower");
        playerRef.SetTower(newTower);
  }
}
