using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityStandardAssets.Characters.ThirdPerson.PunDemos;
using Photon.Realtime;

public class CreateOrJoinLobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private float refreshInterval = 5f; // Opdateringsinterval i sekunder
    public TMP_InputField createRoomInputField;
    private GameObject _content;
    // Remember that when creating a room, the player automatically and immediately joins that room
    public void CreateRoom()
    {
        Debug.Log("Creating room");
        PhotonNetwork.CreateRoom(createRoomInputField.text, new Photon.Realtime.RoomOptions {PublishUserId = true});
    }

    void Start(){
        Debug.Log("Start");
        RefreshRoomList();
    }
    public void RefreshRoomList(){
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
        Debug.Log("Room list refreshed");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene"); // Load room scene instead of game scene
        Debug.Log("Room joined from function");
    }

}