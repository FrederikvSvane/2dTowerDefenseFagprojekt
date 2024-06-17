using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public int[] _playerHealthValues;

    public void InitPlayerHealthValues()
    {
        _playerHealthValues = new int[PhotonNetwork.PlayerList.Length];
        for (int i = 0; i < _playerHealthValues.Length; i++)
        {
            _playerHealthValues[i] = 100;
        }
    }

    public void TransferHealthValues(int givingPlayer, int recievingPlayer, int healthValue)
    {
        _playerHealthValues[givingPlayer] -= healthValue;
        _playerHealthValues[recievingPlayer] += healthValue;
    }
}
