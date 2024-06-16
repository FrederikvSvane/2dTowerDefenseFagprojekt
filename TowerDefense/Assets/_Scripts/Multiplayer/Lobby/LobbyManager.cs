 using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
    private List<Photon.Realtime.Player> _readyPlayers = new List<Photon.Realtime.Player>();
    private bool alreadyPressed = false;

    public void Awake(){
        GetCurrentRoomPlayers();
    }
    void Start()
    {
        // Check and print the first player who joined the room
        Photon.Realtime.Player firstPlayer = GetFirstPlayer();
        if (firstPlayer != null)
        {
            Debug.Log("First player who joined the room: " + firstPlayer.NickName);
        }
    }

    private Photon.Realtime.Player GetFirstPlayer()
    {
        // Get the list of players in the order they joined
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        if (players.Length > 0)
        {
            return players[0]; // The first player in the list
        }
        return null;
    }

    private void GetCurrentRoomPlayers(){
        foreach(KeyValuePair<int, Photon.Realtime.Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerInfo.Value);
        }
    }

    private void AddPlayerListing(Photon.Realtime.Player player){
        PlayerListing listing = Instantiate(_playerListing, _content);
        listing.name = player.UserId;
        Debug.Log("Listing Name is: " + listing.name);
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
    
    [PunRPC]
    public void AddPlayerReady(Photon.Realtime.Player player){
        if (!_readyPlayers.Contains(player))
        {
            _readyPlayers.Add(player);
            Debug.Log(player.NickName + "is ready");

            foreach (Photon.Realtime.Player p in _readyPlayers){ // Burde opdatere farven på hver spiller i listen der er ready.
                PlayerListing myIndex =_playerListings.Find(x => x._player == p);

                myIndex.SetTextColor(Color.green);
            }

        } 
    }
    [PunRPC]
    public void RemovePlayerReady(Photon.Realtime.Player player){
        if(_readyPlayers.Contains(player)){
            _readyPlayers.Remove(player);
            Debug.Log(player.NickName + "Was removed");
        }
    }

    public void GameReady(){
        
        if(alreadyPressed == false){
        _photonView.RPC("AddPlayerReady", RpcTarget.All, PhotonNetwork.LocalPlayer);

        
        alreadyPressed = true;
        }else if(alreadyPressed == true){
        _photonView.RPC("RemovePlayerReady", RpcTarget.All, PhotonNetwork.LocalPlayer);

        PlayerListing myIndex =_playerListings.Find(x => x._player == PhotonNetwork.LocalPlayer);

        myIndex.SetTextColor(Color.gray);
        alreadyPressed = false;    
        }
        
    }

    public void StartGame(){
        Photon.Realtime.Player firstPlayer = GetFirstPlayer();
        if (_readyPlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount && firstPlayer == PhotonNetwork.LocalPlayer)
        {
            _photonView.RPC("StartGameRPC", RpcTarget.All);
        }
        
    }
}
