using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameListingManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _content;
    [SerializeField] private GameListing _gameListing;
    private List<GameListing> _listings = new List<GameListing>();

    private int DoesRoomExist(RoomInfo room){
        int index = _listings.FindIndex(x => x._roomInfo.Name == room.Name);
        return index;
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        { 
            int index = DoesRoomExist(room);
            if(room.RemovedFromList){
                if (index != -1){
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
                continue;
            } else if(room.IsVisible && index == -1){
                GameListing listing = Instantiate(_gameListing, _content);
                listing.SetGameInfo(room, room.Name, room.MaxPlayers, room.PlayerCount);
                _listings.Add(listing);
            }
        }
    }
}

