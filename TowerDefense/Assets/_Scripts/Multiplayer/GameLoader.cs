using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameLoader : MonoBehaviour
{
    public GridManager _gridManager;
    public TowerManager _towerManager;
    public TimeManager _timeManager;
    public PlayerManager _playerManager;

    public void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(_towerManager.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(_timeManager.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(_playerManager.name, Vector3.zero, Quaternion.identity);
            PhotonNetwork.Instantiate(_gridManager.name, Vector3.zero, Quaternion.identity);
        }
    }
}
