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
    [SerializeField] private PhotonView _photonView;  
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
        listing.name = player.UserId;
        if (listing != null){
            listing.SetPlayerInfo(player);
            _playerListings.Add(listing);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = _playerListings.FindIndex(x => x._player == otherPlayer);
        if (index != -1){
            Destroy(_playerListings[index].gameObject);
            _playerListings.RemoveAt(index);
        }
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("CreateOrJoinScene");
    }

    [PunRPC]
    public void StartGameRPC(){
        PhotonNetwork.LoadLevel("GameScene");
    }
    
    public void StartGame(){
        _photonView.RPC("StartGameRPC", RpcTarget.All);
    }
}
