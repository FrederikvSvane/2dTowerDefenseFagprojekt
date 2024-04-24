using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayingAs : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private PlayerListing _player;

    // Update is called once per frame
    void Update()
    {
        _player.SetPlayerName(_text.text);
    }
}
