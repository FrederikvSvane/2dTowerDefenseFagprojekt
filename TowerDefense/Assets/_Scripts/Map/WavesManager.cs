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
    Unit regularUnit = new Unit(40f, 10f, 2f); //creating different units Health, damage and speed.
    Unit tankUnit = new Unit(70f, 15f, 1f);
    Unit fastUnit = new Unit(20f, 10f, 5f);

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
    public void initializeWaves(GridManager gridManager){
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        gridManager.SpawnUnitsOnAllMaps(localPlayerId, fastUnit, 5);
        gridManager.SpawnUnitsOnAllMaps(localPlayerId, regularUnit, 10);
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
            while(true){
            int currentTime = timeManager.getMinutes();
            float currentSeconds = timeManager.getSeconds();
            if(currentTime == 1 && sendWave == false){
                
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, regularUnit, 15);
                sendWave = true;
            }else if(currentTime == 2){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, tankUnit, 20);    
            }else if(currentTime == 3){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, tankUnit, 20);    
            }else if(currentTime == 4){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, fastUnit, 20);    
            }else if(currentTime == 5){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, regularUnit, 30);    
            }else if(currentTime == 6){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, tankUnit, 20);    
            }
            yield return new WaitForSeconds(1f);
            }
        } 
    }
}