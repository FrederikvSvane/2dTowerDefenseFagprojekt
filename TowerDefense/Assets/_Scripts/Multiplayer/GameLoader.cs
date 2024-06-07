using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class GameLoader : MonoBehaviour
{
    public GridManager _gridManager;
    public TowerManager _towerManager;
    public TimeManager _timeManager;

    public void Start()
    {
        PhotonNetwork.Instantiate(_gridManager.name, Vector3.zero, Quaternion.identity);
        Debug.Log(_gridManager.name + " initialized");
        PhotonNetwork.Instantiate(_towerManager.name, Vector3.zero, Quaternion.identity);
        Debug.Log(_towerManager.name + " initialized");
        // PhotonNetwork.Instantiate(_timeManager.name, Vector3.zero, Quaternion.identity);
        // Debug.Log(_towerManager.name + " initialized");
    }

    public void Update(){
    }
}
