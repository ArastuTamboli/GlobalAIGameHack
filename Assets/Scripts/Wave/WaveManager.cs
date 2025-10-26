using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    [Header("Wave Configuration")]
    public List<WaveConfig> waves = new List<WaveConfig>();
    public int currentWaveIndex = 0;

    [Header("Spawn Points")]
    public Transform pathASpawnPoint;
    public Transform pathBSpawnPoint;

    [Header("Scaling")]
    public float healthMultiplier = 1.15f;
    public float damageMultiplier = 1.1f;
    [Header("Path Progress Triggers")]
    public PathProgressTrigger[] path1Triggers;
    public PathProgressTrigger[] path2Triggers;
    [Header("References")]
    public PathManager pathManager;
    public GameManager gameManager;

    [Header("Events")]
    public UnityEvent<int> onWaveStart;
    public UnityEvent<int> onWaveCleared;
    public UnityEvent onAllWavesComplete;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private bool allEnemiesSpawned = false;
    private bool isSpawning = false;
    private int currentWaveTotalEnemies = 0;
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (pathManager == null)
            pathManager = PathManager.instance;

        if (gameManager == null)
            gameManager = GameManager.instance;

   
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (currentWaveIndex < waves.Count)
        {
            yield return StartCoroutine(WaveSequence());
            currentWaveIndex++;
        }

        Debug.Log("All waves completed! You Win!");
        onAllWavesComplete?.Invoke();
        gameManager.ShowWinPanel();
    }

    IEnumerator WaveSequence()
    {
        int waveNumber = currentWaveIndex + 1;

        gameManager.ShowWaveText($"Wave {waveNumber}", 2f);
        yield return new WaitForSeconds(2f);
        gameManager.HideWaveText();

        onWaveStart?.Invoke(waveNumber);

        WaveConfig currentWave = waves[currentWaveIndex];
        currentWaveTotalEnemies = currentWave.GetTotalEnemyCount();

        InitializePathTriggers(currentWave.pathAEnemyCount, currentWave.pathBEnemyCount);

        yield return StartCoroutine(SpawnWave(currentWave, waveNumber));

        yield return new WaitUntil(() => AreAllEnemiesDefeated());

        if (IsPaused) yield break;

        Debug.Log($"Wave {waveNumber} Cleared!");
        onWaveCleared?.Invoke(waveNumber);
        gameManager.ShowWaveClearedText();

        yield return new WaitForSeconds(2f);

        if (currentWaveIndex + 1 < waves.Count)
        {
            gameManager.ShowRestPeriod();
            yield return StartCoroutine(RestPeriod());
        }
    }

    void InitializePathTriggers(int path1EnemyCount, int path2EnemyCount)
    {
        if (path1Triggers != null && path1Triggers.Length > 0)
        {
            foreach (PathProgressTrigger trigger in path1Triggers)
            {
                if (trigger != null)
                {
                    trigger.ResetTrigger();
                    trigger.SetTotalEnemies(path1EnemyCount,0);
                    Debug.Log($"<color=green>[Path 1 Trigger]</color> Set to track {path1EnemyCount} enemies");
                }
            }
        }

        if (path2Triggers != null && path2Triggers.Length > 0)
        {
            foreach (PathProgressTrigger trigger in path2Triggers)
            {
                if (trigger != null)
                {
                    trigger.ResetTrigger();
                    trigger.SetTotalEnemies(path2EnemyCount, 1);
                    Debug.Log($"<color=blue>[Path 2 Trigger]</color> Set to track {path2EnemyCount} enemies");
                }
            }
        }
    }

    IEnumerator SpawnWave(WaveConfig wave, int waveNumber)
    {
        isSpawning = true;
        allEnemiesSpawned = false;
        activeEnemies.Clear();

        List<SpawnData> spawnSequence = BuildSpawnSequence(wave, waveNumber);

        foreach (SpawnData spawnData in spawnSequence)
        {
            if (IsPaused)
            {
                isSpawning = false;
                yield break;
            }

            SpawnEnemyOnPath(spawnData);
            yield return new WaitForSeconds(wave.GetRandomSpawnInterval());
        }

        allEnemiesSpawned = true;
        isSpawning = false;
        Debug.Log($"All enemies spawned for Wave {waveNumber}. Total: {activeEnemies.Count}");
    }

    List<SpawnData> BuildSpawnSequence(WaveConfig wave, int waveNumber)
    {
        List<SpawnData> sequence = new List<SpawnData>();

        for (int i = 0; i < wave.pathAEnemyCount; i++)
        {
            EnemyType randomEnemy = wave.GetRandomEnemyType();
            sequence.Add(new SpawnData(randomEnemy, 0, waveNumber));
        }

        for (int i = 0; i < wave.pathBEnemyCount; i++)
        {
            EnemyType randomEnemy = wave.GetRandomEnemyType();
            sequence.Add(new SpawnData(randomEnemy, 1, waveNumber));
        }

        for (int i = sequence.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            SpawnData temp = sequence[i];
            sequence[i] = sequence[randomIndex];
            sequence[randomIndex] = temp;
        }

        return sequence;
    }

    void SpawnEnemyOnPath(SpawnData spawnData)
    {
        Transform[] selectedPath = pathManager.GetPath(spawnData.pathIndex);

        Transform spawnPoint = spawnData.pathIndex == 0 ?
            pathASpawnPoint:
            pathBSpawnPoint;

        GameObject enemyObj = Instantiate(spawnData.enemyType.prefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Initialize(spawnData.enemyType, spawnData.waveNumber, healthMultiplier, damageMultiplier);
            enemy.waypoints = selectedPath;
            enemy.waveManager = this;
            activeEnemies.Add(enemy);
        }
    }

    IEnumerator RestPeriod()
    {
        float restTime = 30f;
        float countdownTime = 10f;

        yield return StartCoroutine(gameManager.ShowCountdown(restTime, "Take Rest"));
        yield return StartCoroutine(gameManager.ShowCountdown(countdownTime, "Next Wave in"));

        gameManager.HideRestPeriod();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Enemy removed. Remaining: {activeEnemies.Count}");
        }
    }

    bool AreAllEnemiesDefeated()
    {
        return allEnemiesSpawned && activeEnemies.Count == 0;
    }

    public void PauseSpawning()
    {
        IsPaused = true;
        StopAllCoroutines();

        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null)
            {
                enemy.StopMovement();
            }
        }
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    public int GetCurrentWaveTotalEnemies()
    {
        return currentWaveTotalEnemies;
    }

    private class SpawnData
    {
        public EnemyType enemyType;
        public int pathIndex;
        public int waveNumber;

        public SpawnData(EnemyType type, int path, int wave)
        {
            enemyType = type;
            pathIndex = path;
            waveNumber = wave;
        }
    }
}
