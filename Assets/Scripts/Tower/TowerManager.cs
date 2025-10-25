using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    public GameObject[] towerPrefabs;

    private Dictionary<Vector3, GameObject> placedTowers = new Dictionary<Vector3, GameObject>();
    int TowerACount = 0;
    int TowerBCount = 0;
    int TowerCCount = 0;
    int TowerDCount = 0;
    int TowerECount = 0;
    public void PlaceTower(string towerType, Vector3 position)
    {
        towerType = towerType.ToLower();

        if (placedTowers.ContainsKey(position))
        {
            Debug.LogWarning($"Tower already exists at position {position}");
            return;
        }
        int count = -1;
        if (towerType == "A")
        {
            TowerACount++;
            count = TowerACount;
        }
        else if(towerType == "B")
        {
            TowerBCount++;
            count = TowerBCount;
        }
         else if(towerType == "C")
        {
            TowerCCount++;
            count = TowerCCount;
        }
        else if (towerType == "D")
        {
            TowerDCount++;
            count = TowerDCount;
        }
        else if (towerType == "E")
        {
            TowerECount++;
            count = TowerECount;
        }
        //NeocortexTowerObject towerPrefab = towerPrefabs.Find(t => t.Name.ToLower() == towerType).TowerPrefab;

        //if (towerPrefab != null)
        //{
        //    NeocortexTowerObject tower = Instantiate(towerPrefab, position, Quaternion.identity, transform);
        //    placedTowers[position] = tower.gameObject;
        //    tower.Init($"{towerType}{count}", towerType);
        //    tower.name = $"{towerType}{count}";  
        //    Debug.Log($"Placed {towerType} at {position}");
        //}
        //else
        //{
        //    Debug.LogWarning($"Tower type {towerType} not found in prefabs list");
        //}
    }

    public void RemoveTower(Vector3 position)
    {
        if (placedTowers.TryGetValue(position, out GameObject tower))
        {
            Destroy(tower);
            placedTowers.Remove(position);
        }
    }

    public void ClearAllTowers()
    {
        foreach (var tower in placedTowers.Values)
        {
            Destroy(tower);
        }
        placedTowers.Clear();
    }
  
}
