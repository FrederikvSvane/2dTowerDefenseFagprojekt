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
        gridManager.SpawnUnitsOnAllMaps(localPlayerId, 30f, 20f, 10);
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
                
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, 50f, 20f, 15);
                sendWave = true;
            }else if(currentTime == 2){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, 60f, 20f, 20);    
            }else if(currentTime == 3){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, 100f, 20f, 20);    
            }else if(currentTime == 4){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, 150f, 20f, 20);    
            }else if(currentTime == 5){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId, 160f, 20f, 30);    
            }else if(currentTime == 6){
                gridManager.SpawnUnitsOnAllMaps(localPlayerId   , 400f, 20f, 20);    
            }
            yield return new WaitForSeconds(1f);
            }
        } 
    }
}