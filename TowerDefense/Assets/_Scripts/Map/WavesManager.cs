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
            DontDestroyOnLoad(gameObject); // Optional if you want the object to persist across scene changes
        }
    }

    public void setPlayerMap(Dictionary<int, Photon.Realtime.Player> incMap){
        this.playerMap = incMap;
    }
    
    public void initializeWaves(GridManager gridManager){
        gridManager.SpawnUnitsDynamicPosition(playerMap);
        timeManager = FindObjectOfType<TimeManager>();

        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found");
            return;
        }
        StartCoroutine(startWaves());
        IEnumerator startWaves()
        {
            while(true){
            int currentTime = timeManager.getMinutes();
            if(currentTime < 0){
                Debug.Log("Time is less than a minute");
            }else if(currentTime > 0){
                Debug.Log("Time is more than a minute");
            }
            yield return new WaitForSeconds(2f);
            }
        } 
    }
}