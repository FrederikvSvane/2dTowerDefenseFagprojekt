using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Photon.Pun;
using UnityEngine;

public class PlayerLivesManager : MonoBehaviourPun
{
    // Start is called before the first frame update
    private List<Photon.Realtime.Player> _players = new List<Photon.Realtime.Player>();
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _content;
    private int _playerCount = 0;
    void Start()
    {
        /*
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.IsLocal){
                Debug.Log(player.NickName);
                _players.Add(player);
                GameObject pl = Instantiate(_playerPrefab, new Vector3(-270, -(30 + (90 * _playerCount)), 0), Quaternion.identity);
                pl.transform.SetParent(_content.transform, false);
                pl.GetComponent<PlayerLife>().SetPlayer(player);
                _playerCount++;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
