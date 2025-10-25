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
    public int waveNumber;
    public List<EnemySpawnInfo> enemySpawns = new List<EnemySpawnInfo>();

    [Header("Timing")]
    public float timeBetweenSpawns = 0.5f;
    public float delayBeforeNextWave = 5f;
}
