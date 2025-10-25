using System.Collections;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int money = 500;

    [Header("References")]
    public WaveManager waveManager;
    public UIManager uiManager;

    private bool isGameOver = false;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        currentHealth = maxHealth;

        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();

        UpdateUI();

        if (waveManager != null)
        {
            waveManager.onWaveStart.AddListener(OnWaveStart);
            waveManager.onWaveCleared.AddListener(OnWaveCleared);
        }
    }

    void Update()
    {
        UpdateEnemyCount();
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    void OnWaveStart(int waveNumber)
    {
        Debug.Log($"Wave {waveNumber} Started");
    }

    void OnWaveCleared(int waveNumber)
    {
        AddMoney(50 + (waveNumber * 10));
    }

    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateHealth(currentHealth, maxHealth);
            uiManager.UpdateMoney(money);
        }
    }

    void UpdateEnemyCount()
    {
        if (uiManager != null && waveManager != null)
        {
            int count = waveManager.GetActiveEnemyCount();
            uiManager.UpdateEnemyCount(count);
        }
    }

    public void ShowWaveText(string text, float duration)
    {
        if (uiManager != null)
            uiManager.ShowWaveText(text);
    }

    public void HideWaveText()
    {
        if (uiManager != null)
            uiManager.HideWaveText();
    }

    public void ShowWaveClearedText()
    {
        if (uiManager != null)
            uiManager.ShowWaveClearedText();
    }

    public void ShowRestPeriod()
    {
        if (uiManager != null)
            uiManager.ShowRestPeriod();
    }

    public void HideRestPeriod()
    {
        if (uiManager != null)
            uiManager.HideRestPeriod();
    }

    public IEnumerator ShowCountdown(float duration, string label)
    {
        if (uiManager != null)
        {
            yield return StartCoroutine(uiManager.ShowCountdown(duration, label));
        }
    }

    public void ShowWinPanel()
    {
        isGameOver = true;
        if (uiManager != null)
        {
            uiManager.ShowWinPanel();
        }
        Time.timeScale = 0;
    }

    void GameOver()
    {
        isGameOver = true;

        if (waveManager != null)
        {
            waveManager.PauseSpawning();
        }

        if (uiManager != null)
        {
            uiManager.ShowLosePanel();
        }

        Time.timeScale = 0;
        Debug.Log("Game Over - You Lost!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        Debug.Log("Game Quit");
    }
}
