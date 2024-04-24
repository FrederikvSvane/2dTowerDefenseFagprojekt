using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviourPunCallbacks 
{
    [SerializeField] private Transform _content;
    [SerializeField] private PlayerListing _playerListing;  
    private List<PlayerListing> _playerListings = new List<PlayerListing>();

    public void Awake(){
        GetCurrentRoomPlayers();
    }

    private void GetCurrentRoomPlayers(){
        foreach(KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Photon.Realtime.Player player){
        PlayerListing listing = Instantiate(_playerListing, _content);
        if (listing != null){
            listing.SetPlayerInfo(player);
            _playerListings.Add(listing);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player joined room from LobMan");
        AddPlayerListing(newPlayer);
    }
}
