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
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomInputField.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomInputField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene"); // Load room scene instead of game scene
    }
}
