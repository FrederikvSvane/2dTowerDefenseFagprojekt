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
    [SerializeField] private Transform _content;
    [SerializeField] private PlayerListing _playerListing;  
    private List<PlayerListing> _playerListings = new List<PlayerListing>();

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player joined room from LobMan");
        PlayerListing playerListing = Instantiate(_playerListing, _content);
        if (playerListing != null)
        {
            playerListing.SetPlayerInfo(newPlayer);
            _playerListings.Add(playerListing);
        }
    }

}
