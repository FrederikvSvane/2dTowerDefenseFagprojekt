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
    public GameObject playerTextPrefab;

    public PlayerManager _playerManager;



    void Start(){
        
        Debug.Log(PhotonNetwork.PlayerList.Length);
        OnJoinedRoom();
    }
    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void UpdatePlayerList()
    {
        Debug.Log("Updating player list");
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList){
            playerList.Add("Player " + _playerManager.GetPlayerNumber(player.UserId));
        }

        foreach(Transform child in contentArea){
            Destroy(child.gameObject);
        }

        foreach(string playername in playerList){
            GameObject player = Instantiate(playerTextPrefab, contentArea);
            player.GetComponent<TMP_Text>().text = playername;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        _playerManager = FindObjectOfType<PlayerManager>();
        UpdatePlayerList();
        PhotonView photonView = FindObjectOfType<PhotonView>();
        photonView.RPC("UpdatePlayerList", RpcTarget.Others);
    }

    
}
