using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameListing : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _maxPlayersText;
    public RoomInfo _roomInfo { get; private set; }

    public void SetGameInfo(RoomInfo roomInfo, string gameName, int _maxPlayers, int _currentPlayers)
    {
        _roomInfo = roomInfo;
        _text.text = gameName;
        _maxPlayersText.text = _currentPlayers + "/" + _maxPlayers;
    }

    public void JoinRoom()
    {
        Debug.Log("Join room");
        PhotonNetwork.JoinRoom(_text.text); //Join room
    }
}
