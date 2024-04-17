using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CreateOrJoinLobby : MonoBehaviourPunCallbacks
{

    public TMP_InputField createRoomInputField;
    public TMP_InputField joinRoomInputField;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
        PhotonNetwork.LoadLevel("GameScene"); // Load room scene instead of game scene
    }
}
