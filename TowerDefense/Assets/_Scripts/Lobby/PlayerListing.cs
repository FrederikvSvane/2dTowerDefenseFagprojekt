using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    
    public Photon.Realtime.Player _player { get; private set; } 

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {
        _player = player;
        _text.text = player.UserId;
    }

}
