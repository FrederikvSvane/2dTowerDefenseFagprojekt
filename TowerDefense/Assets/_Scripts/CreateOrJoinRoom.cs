using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityStandardAssets.Characters.ThirdPerson.PunDemos;

public class CreateOrJoinLobby : MonoBehaviourPunCallbacks
{

    public TMP_InputField createRoomInputField;
    public TMP_InputField joinRoomInputField;
    public GameObject _lobbyLoader;
    public PlayerManager _playerManager;

    // Remember that when creating a room, the player automatically and immediately joins that room
    public void Start()
    {
        _playerManager = Instantiate(_playerManager);
    }
    public void CreateRoom()
    {
        Debug.Log("Creating room");
        PhotonNetwork.CreateRoom(createRoomInputField.text);

    }

    public void JoinRoom()
    {
        Debug.Log("Join room");
        PhotonNetwork.JoinRoom(joinRoomInputField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene"); // Load room scene instead of game scene
        Debug.Log("Room joined from function");

    }
}
