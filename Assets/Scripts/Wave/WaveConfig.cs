using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyType enemyType;
    public int count;
    public float spawnInterval = 0.5f;
}

[CreateAssetMenu(fileName = "New Wave", menuName = "Tower Defense/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [Header("Wave Info")]
    public string waveName = "Wave 1";

    [Header("Enemy Types Pool")]
    public EnemyType[] enemyTypes;

    [Header("Path Distribution")]
    public int pathAEnemyCount = 5;
    public int pathBEnemyCount = 5;

    [Header("Spawn Timing")]
    public float spawnIntervalMin = 1f;
    public float spawnIntervalMax = 2f;

    public int GetTotalEnemyCount()
    {
        return pathAEnemyCount + pathBEnemyCount;
    }

    public EnemyType GetRandomEnemyType()
    {
        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogError($"No enemy types assigned to wave {waveName}!");
            return null;
        }

        return enemyTypes[Random.Range(0, enemyTypes.Length)];
    }
    public float GetRandomSpawnInterval()
    {
        
        return Random.Range(spawnIntervalMin, spawnIntervalMax);
    }
}
