using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameLoader : MonoBehaviour
{
    public GameObject _gameObject;

    public void Start()
    {
        PhotonNetwork.Instantiate(_gameObject.name, Vector3.zero, Quaternion.identity);
    }
}
