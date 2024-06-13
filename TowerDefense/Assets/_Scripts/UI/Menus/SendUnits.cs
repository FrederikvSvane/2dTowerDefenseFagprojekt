using Photon.Pun;
using UnityEngine;

public class SendUnits : MonoBehaviourPun
{
    private GridManager _gridManager;
    private void Awake()
    {
        _gridManager = GridManager.Instance;
        Debug.Log("SendUnits Awake");
        if(_gridManager == null)
        {
            Debug.LogError("GridManager is null in SendUnits");
        }
    }

    public void SendUnitsToNextAlivePlayer()
    {
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        int nextPlayerId = _gridManager.GetNextAlivePlayerId(localPlayerId);

        // If an alive player is found, call the RPC function to spawn units on their map
        if (nextPlayerId != -1)
        {
            _gridManager._photonView.RPC("SpawnUnitsOnMyMap", PhotonNetwork.CurrentRoom.Players[nextPlayerId], nextPlayerId);
        }
    }

    [PunRPC]
    private void SpawnUnitsOnMyMap(int playerId)
    {
        StartCoroutine(_gridManager.SpawnUnit(playerId));
    }
}