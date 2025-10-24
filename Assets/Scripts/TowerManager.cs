using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [System.Serializable]
    public class TowerPrefab
    {
        public string towerName;
        public GameObject prefab;
    }

    [Header("Tower Prefabs")]
    public List<TowerPrefab> towerPrefabs = new List<TowerPrefab>();

    private Dictionary<Vector3, GameObject> placedTowers = new Dictionary<Vector3, GameObject>();

    public void PlaceTower(string towerType, Vector3 position)
    {
        towerType = towerType.ToLower();

        if (placedTowers.ContainsKey(position))
        {
            Debug.LogWarning($"Tower already exists at position {position}");
            return;
        }

        TowerPrefab towerPrefab = towerPrefabs.Find(t => t.towerName.ToLower() == towerType);

        if (towerPrefab != null && towerPrefab.prefab != null)
        {
            GameObject tower = Instantiate(towerPrefab.prefab, position, Quaternion.identity, transform);
            placedTowers[position] = tower;
            Debug.Log($"Placed {towerType} at {position}");
        }
        else
        {
            Debug.LogWarning($"Tower type {towerType} not found in prefabs list");
        }
    }

    public void RemoveTower(Vector3 position)
    {
        if (placedTowers.TryGetValue(position, out GameObject tower))
        {
            Destroy(tower);
            placedTowers.Remove(position);
        }
    }
}
