using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviourPunCallbacks 
{
    // Start is called before the first frame update
    [SerializeField] private Transform contentArea;
    private List<string> playerList = new List<string>();
    private int newID = 1;
    public GameObject playerTextPrefab;

    private Dictionary<string, string> playerIdD = new Dictionary<string, string>();

    void Start(){

        Debug.Log(PhotonNetwork.PlayerList.Length);
        updatePlayerList();
    }
    // Update is called once per frame
    void Update()
    {
    }
    void updatePlayerList()
    {
        
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList){
            Debug.Log(player.UserId);
            playerIdD.Add(player.UserId, newID.ToString());
            playerList.Add("Player " + playerIdD[player.UserId]);
        }

        foreach(Transform child in contentArea){
            Destroy(child.gameObject);
        }

        foreach(string playername in playerList){
            GameObject player = Instantiate(playerTextPrefab, contentArea);
            player.GetComponent<TMP_Text>().text = playername;
        }
    }

    
}
