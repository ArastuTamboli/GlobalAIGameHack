using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int money = 500;
    [Header("Events")]
    public UnityEvent<int> onCurrencyChanged;
    public UnityEvent onFirstDamage;
    public UnityEvent onAnyDamage;
    public UnityEvent<float> onHealthThreshold50;
    public UnityEvent<float> onHealthThreshold20;
    [Header("References")]
    public WaveManager waveManager;
    public UIManager uiManager;

    private bool hasTriggeredFirstDamage = false;
    private bool hasTriggered50Threshold = false;
    private bool hasTriggered20Threshold = false;
    private bool isGameOver = false;

    public GameObject hitEffect;
    public GameObject gateHitPos;
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
        MusicManager.Instance?.PlayGameplayMusic();
    }

    void Update()
    {
        UpdateEnemyCount();
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;
        bool wasFullHealth = (currentHealth == maxHealth);
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Instantiate(hitEffect, gateHitPos.transform.position, Quaternion.identity);

        float healthPercentage = (float)currentHealth / maxHealth;
        onAnyDamage?.Invoke();

        if (CameraShake.Instance != null)
        {
            float shakeIntensity = Mathf.Lerp(0.1f, 0.4f, 1f - healthPercentage);
            CameraShake.Instance.TriggerShake(0.3f, shakeIntensity);
        }

        if (DamagePostProcessing.Instance != null)
        {
            float effectIntensity = Mathf.Lerp(0.8f, 2f, 1f - healthPercentage);
            DamagePostProcessing.Instance.TriggerDamageEffect(effectIntensity);
        }
        if (wasFullHealth && !hasTriggeredFirstDamage)
        {
            hasTriggeredFirstDamage = true;
            Debug.Log("<color=red> FIRST DAMAGE TAKEN!</color>");
            onFirstDamage?.Invoke();
        }

        if (healthPercentage <= 0.5f && !hasTriggered50Threshold)
        {
            hasTriggered50Threshold = true;
            Debug.Log("<color=orange> CRITICAL: Gate at 50% health!</color>");
            onHealthThreshold50?.Invoke(healthPercentage);
        }

        if (healthPercentage <= 0.2f && !hasTriggered20Threshold)
        {
            hasTriggered20Threshold = true;
            Debug.Log("<color=red> EMERGENCY: Gate at 20% health!</color>");
            onHealthThreshold20?.Invoke(healthPercentage);
        }

        UpdateUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddMoney(int amount)
    {
        money += amount;
        onCurrencyChanged?.Invoke(money);
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            onCurrencyChanged?.Invoke(money);
            UpdateUI();
            return true;
        }
        return false;
    }
    // Update any existing currency changes to trigger the event
    public void SetCurrency(int amount)
    {
        money = amount;
        onCurrencyChanged?.Invoke(money);
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
