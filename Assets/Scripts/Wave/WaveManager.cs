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
    public Transform[] path1SpawnPoints;
    public Transform[] path2SpawnPoints;

    [Header("Scaling")]
    public float healthMultiplier = 1.15f;
    public float damageMultiplier = 1.1f;

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

        yield return StartCoroutine(SpawnWave(waves[currentWaveIndex], waveNumber));

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

    IEnumerator SpawnWave(WaveConfig wave, int waveNumber)
    {
        isSpawning = true;
        allEnemiesSpawned = false;
        activeEnemies.Clear();

        foreach (EnemySpawnInfo spawnInfo in wave.enemySpawns)
        {
            for (int i = 0; i < spawnInfo.count; i++)
            {
                if (IsPaused)
                {
                    isSpawning = false;
                    yield break;
                }

                SpawnEnemy(spawnInfo.enemyType, waveNumber);
                yield return new WaitForSeconds(spawnInfo.spawnInterval);
            }

            yield return new WaitForSeconds(wave.timeBetweenSpawns);
        }

        allEnemiesSpawned = true;
        isSpawning = false;
        Debug.Log($"All enemies spawned for Wave {waveNumber}. Total: {activeEnemies.Count}");
    }

    IEnumerator RestPeriod()
    {
        float restTime = 30f;
        float countdownTime = 10f;

        yield return StartCoroutine(gameManager.ShowCountdown(restTime, "Rest Period"));

        yield return StartCoroutine(gameManager.ShowCountdown(countdownTime, "Next Wave in"));

        gameManager.HideRestPeriod();
    }

    void SpawnEnemy(EnemyType enemyType, int waveNumber)
    {
        int pathIndex = pathManager.GetRandomPathIndex();
        Transform[] selectedPath = pathManager.GetPath(pathIndex);

        Transform spawnPoint = pathIndex == 0 ?
            path1SpawnPoints[Random.Range(0, path1SpawnPoints.Length)] :
            path2SpawnPoints[Random.Range(0, path2SpawnPoints.Length)];

        GameObject enemyObj = Instantiate(enemyType.prefab, spawnPoint.position, Quaternion.identity);
        Enemy enemy = enemyObj.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.Initialize(enemyType, waveNumber, healthMultiplier, damageMultiplier);
            enemy.waypoints = selectedPath;
            enemy.waveManager = this;
            activeEnemies.Add(enemy);
        }
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
}
