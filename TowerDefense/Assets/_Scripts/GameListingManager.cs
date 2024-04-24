using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameListingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _content;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList){
            Debug.Log(room);
        }
    }
}
