using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TowerManager : MonoBehaviour, IPunInstantiateMagicCallback
{
    private Dictionary<string, GameObject> towerPrefabDict;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        InitializeTowerPrefabDict();
    }

    private void InitializeTowerPrefabDict()
    {
        towerPrefabDict = new Dictionary<string, GameObject>();

        // Load the tower prefabs from Resources folder
        GameObject rangedTowerPrefab = Resources.Load<GameObject>("Tower");
        // Add more tower prefabs as needed

        // Add the tower prefabs to the dictionary
        towerPrefabDict.Add("RangedTower", rangedTowerPrefab);
        // Add more tower types and their corresponding prefabs to the dictionary
    }

    public GameObject GetTowerPrefab(string towerType)
    {
        if (towerPrefabDict.TryGetValue(towerType, out GameObject towerPrefab))
        {
            return towerPrefab;
        }
        else
        {
            Debug.LogError($"Tower prefab not found for type: {towerType}");
            return null;
        }
    }
}