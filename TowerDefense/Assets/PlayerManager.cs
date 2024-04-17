using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPunInstantiateMagicCallback
{
    private Dictionary<string, string> playerIdD = new Dictionary<string, string>();
    private int playerNumber = 1;
    // Start is called before the first frame update
    public void AddPlayerToDictionary(string UserId){
        playerIdD.Add(UserId, playerNumber.ToString());
        playerNumber++;
    }

    public Dictionary<string, string> GetPlayerDictionary(){
        return playerIdD;
    }

    public string GetPlayerNumber(string UserId){
        return playerIdD[UserId];
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
