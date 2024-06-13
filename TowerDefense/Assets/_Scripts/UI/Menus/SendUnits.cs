using Photon.Pun;
using UnityEngine;

public class SendUnits : MonoBehaviourPun
{
    private GridManager _gridManager;
    private PlayerManager _playerManager;

    private void Awake()
    {
        // Access GridManager instance through singleton pattern
        _gridManager = GridManager.Instance;
        if (_gridManager == null)
        {
            Debug.LogError("GridManager instance is not found!");
        }
    }

    public void SendUnitsToNextAlivePlayer()
    {
        if (_gridManager == null)
        {
            Debug.LogError("Cannot send units because GridManager is null.");
            return;
        }

        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        int nextPlayerId = _gridManager.GetNextAlivePlayerId(localPlayerId); //returns -1 if error

        if (nextPlayerId != -1)
        {
            _gridManager._photonView.RPC("SpawnUnitsOnMyMap", PhotonNetwork.CurrentRoom.Players[nextPlayerId], nextPlayerId);
        }else if(nextPlayerId == -1)
        {
            Debug.LogError("No more players left to send units to.");
        }
    }

    [PunRPC]
    private void SpawnUnitsOnMyMap(int playerId)
    {
        if (_gridManager == null)
        {
            Debug.LogError("Cannot spawn units because GridManager is null.");
            return;
        }

        StartCoroutine(_gridManager.SpawnUnit(playerId));
    }
}
