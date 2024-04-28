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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        { 
            if(room.RemovedFromList){
                int index = _listings.FindIndex(x => x._roomInfo.Name == room.Name);
                if (index != -1){
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
                continue;
            } else if(room.IsVisible){
                GameListing listing = Instantiate(_gameListing, _content);
                listing.SetGameInfo(room, room.Name, room.MaxPlayers, room.PlayerCount);
            } else if(!room.IsVisible){
                Debug.Log("Found non visible room");
            }
        }
    }
}

