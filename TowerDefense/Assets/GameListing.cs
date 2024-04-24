using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameListing : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    public RoomInfo _roomInfo { get; private set; }

    public void SetGameInfo(RoomInfo roomInfo, string gameName)
    {
        _roomInfo = roomInfo;
        _text.text = gameName;
    }
}
