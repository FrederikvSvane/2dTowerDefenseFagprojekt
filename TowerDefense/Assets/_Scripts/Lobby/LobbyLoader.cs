using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LobbyLoader : MonoBehaviour
{
    public PlayerManager _playerManager;

    public void Start()
    {
        PhotonNetwork.Instantiate(_playerManager.name, Vector3.zero, Quaternion.identity);
        Debug.Log(_playerManager.name + " initialized");
    }
}
