using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

public class SendUnits : MonoBehaviourPun
{
    private GridManager _gridManager;
    private PlayerManager _playerManager;
    private TimeManager _timeManager;
    private float factor;


    public void SendUnitsToNextAlivePlayer(Unit unit)
    {
        _timeManager = FindObjectOfType<TimeManager>();
        if(unit.name == "Tank Unit"){
            factor = 1.5f;
            _timeManager.IncreasePassiveIncome(factor);
        }else if(unit.name == "Normal Unit"){
            factor = 1f;
            _timeManager.IncreasePassiveIncome(factor);
        }else if(unit.name == "Fast Unit"){
            factor = 0.8f;
            _timeManager.IncreasePassiveIncome(factor);
        }else if(unit.name == "Flying Unit"){
            factor = 1.3f;
            _timeManager.IncreasePassiveIncome(factor);
        }
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
