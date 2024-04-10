using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DummyBoard : MonoBehaviour, IPunInstantiateMagicCallback
{
    public GameObject _enemyPrefab;
    public Tile _tilePrefab;
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("Instantiated");
        // Instantiate(_gameObject, Vector3.zero, Quaternion.identity);
    }
}
