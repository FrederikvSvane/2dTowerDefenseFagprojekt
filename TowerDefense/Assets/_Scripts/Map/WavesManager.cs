using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;


public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set; }
    private Dictionary<int, Photon.Realtime.Player> playerMap;
    private TimeManager timeManager;
    string _regularUnit = "Unit"; //creating different units Health, damage and speed.
    string _tankUnit = "Tank Unit";
    string _fastUnit = "Fast Unit";
    string _flyingUnit = "Flying Unit";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }
    public void InitializeWaves(GridManager gridManager){
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        gridManager.SpawnUnitsOnAllMaps(localPlayerId, _regularUnit, 5);
        gridManager.SpawnUnitsOnAllMaps(localPlayerId, _flyingUnit, 10);
    
        timeManager = FindObjectOfType<TimeManager>();
        
        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found");
            return;
        }
        StartCoroutine(startWaves());
        IEnumerator startWaves()
        {
            bool sendWave = false;
            bool sendWave2 = false;
            while(true){
            int currentTime = timeManager.getMinutes();
            if(currentTime == 1 && sendWave == false){
                
               
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _flyingUnit, 5);
                sendWave = true;
            }else if(currentTime == 2 && sendWave2 == false){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _flyingUnit, 20);   
                sendWave = false;
                sendWave2 = true; 
            }else if(currentTime == 3 && sendWave == false){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _tankUnit, 20);
                sendWave = true;
                sendWave2 = false;    
            }else if(currentTime == 4){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _regularUnit, 20);    
            }else if(currentTime == 5){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _regularUnit, 30);    
            }else if(currentTime == 6){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, _tankUnit, 20);    
            }
            yield return new WaitForSeconds(1f);
            }
        } 
    }
}