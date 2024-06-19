using Photon.Pun;
using UnityEngine;

public class SendUnits : MonoBehaviourPun
{
    private GridManager _gridManager;

    public void SendUnitsToNextAlivePlayer(Unit unit)
    {
        _gridManager = FindObjectOfType<GridManager>();
        int localPlayerId = PhotonNetwork.LocalPlayer.ActorNumber;
        int nextPlayerId = _gridManager.GetNextAlivePlayerId(localPlayerId); //returns -1 if error

        if (nextPlayerId != -1)
        {
            _gridManager._photonView.RPC("SpawnUnitsOnMyMap", PhotonNetwork.CurrentRoom.Players[nextPlayerId], nextPlayerId, unit.name, 1);
            Debug.Log("Ran RPC to send units to player " + nextPlayerId + ".");
        }
        else if (nextPlayerId == -1)
        {
            Debug.LogError("No more players left to send units to.");
        }
    }
}
