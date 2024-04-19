using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPunInstantiateMagicCallback
{
        
    private Dictionary<string, string> _playerDic;
    private int _playerNumber = 1;
    // Start is called before the first frame update
    public void AddPlayerToDictionary(string UserId){
        _playerDic.Add(UserId, _playerNumber.ToString());
        _playerNumber++;
    }

    public Dictionary<string, string> GetPlayerDictionary(){
        return _playerDic;
    }

    public string GetPlayerNumber(string UserId){
        Debug.Log("User GetPlayer");
        Debug.Log(_playerDic);
        return _playerDic[UserId];
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
