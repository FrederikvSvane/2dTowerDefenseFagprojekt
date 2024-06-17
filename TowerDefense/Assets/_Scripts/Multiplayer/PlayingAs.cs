using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayingAs : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _text;
    private Photon.Realtime.Player _player;

    public void Awake(){
        _player = PhotonNetwork.LocalPlayer;
    }
    void Update()
    {
        _player.NickName = _text.text;
    }
}
