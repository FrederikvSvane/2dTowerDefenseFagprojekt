using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
        
    private Dictionary<string /*GUID*/, int /*Assigned Number*/> _playerNumbers;
    private Dictionary<string /*GUID*/, string /*Chosen Name*/> _playerNames;

    void Awake() {
        _playerNumbers = new Dictionary<string, int>();
    }

    public string GetPlayerName(string userId){
        return _playerNames[userId];
    }

    public Dictionary<string, int> GetPlayerDictionary(){
        return _playerNumbers;
    }

    public int GetPlayerNumber(string UserId){
        Debug.Log("User GetPlayer");
        Debug.Log(_playerNumbers);
        if (_playerNumbers.ContainsKey(UserId)) {
        return _playerNumbers[UserId];
            } else {
        Debug.LogError("Player with UserId " + UserId + " not found in _playerNumbers dictionary");
        return -1; // or some other default value
            }
    }

    public void AddPlayerToPlayerNumbers(string UserId){
        _playerNumbers.Add(UserId, _playerNumbers.Count + 1);
    } 

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
