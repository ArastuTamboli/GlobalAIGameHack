using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public Slider healthBar;
    public Image healthBarFill;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI enemyCountText;

    [Header("Health Bar Colors")]
    public Color healthHighColor = Color.green;
    public Color healthMediumColor = Color.yellow;
    public Color healthLowColor = Color.red;

    [Header("Wave Info Panel")]
    public GameObject wavePanel;
    public TextMeshProUGUI waveText;

    [Header("Wave Cleared Panel")]
    public GameObject waveClearedPanel;
    public TextMeshProUGUI waveClearedText;

    [Header("Rest Period Panel")]
    public GameObject restPanel;
    public TextMeshProUGUI restTimerText;
    public TextMeshProUGUI countdownText;

    [Header("End Game Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Buttons")]
    public Button winRestartButton;
    public Button winQuitButton;
    public Button loseRestartButton;
    public Button loseQuitButton;

    
    GameManager gameManager;
    [Header("Tower Shop")]
    public GameObject towerShopPanel;
    public Button towerShopToggleButton;
    void Start()
    {
        gameManager = GameManager.instance;
        if (healthBarFill == null && healthBar != null)
        {
            healthBarFill = healthBar.fillRect.GetComponent<Image>();
        }
        if (towerShopToggleButton != null)
        {
            towerShopToggleButton.onClick.AddListener(ToggleTowerShop);
        }
        SetupButtons();
        HideAllPanels();
    }
    public void ToggleTowerShop()
    {
        if (towerShopPanel != null)
        {
            towerShopPanel.SetActive(!towerShopPanel.activeSelf);
        }
    }
    void SetupButtons()
    {
        if (winRestartButton != null)
            winRestartButton.onClick.AddListener(() => gameManager.RestartGame());

        if (winQuitButton != null)
            winQuitButton.onClick.AddListener(() => gameManager.QuitGame());

        if (loseRestartButton != null)
            loseRestartButton.onClick.AddListener(() => gameManager.RestartGame());

        if (loseQuitButton != null)
            loseQuitButton.onClick.AddListener(() => gameManager.QuitGame());
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;

            float healthPercent = (float)current / max;
            UpdateHealthBarColor(healthPercent);
        }

        //if (healthValueText != null)
        //{
        //    healthValueText.text = $"{current}/{max}";
        //}
    }

    void UpdateHealthBarColor(float healthPercent)
    {
        if (healthBarFill == null) return;

        if (healthPercent > 0.6f)
        {
            healthBarFill.color = healthHighColor;
        }
        else if (healthPercent > 0.3f)
        {
            healthBarFill.color = healthMediumColor;
        }
        else
        {
            healthBarFill.color = healthLowColor;
        }
    }

    public void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = $"${amount}";
    }

    public void UpdateEnemyCount(int count)
    {
        if (enemyCountText != null)
            enemyCountText.text = $"Enemies: {count}";
    }

    public void ShowWaveText(string text)
    {
        if (wavePanel != null)
        {
            wavePanel.SetActive(true);
            if (waveText != null)
                waveText.text = text;
        }
    }

    public void HideWaveText()
    {
        if (wavePanel != null)
            wavePanel.SetActive(false);
    }

    public void ShowWaveClearedText(string text = "Wave Cleared!")
    {
        if (waveClearedPanel != null)
        {
            waveClearedPanel.SetActive(true);
            if (waveClearedText != null)
                waveClearedText.text = text;
        }
    }

    public void HideWaveClearedText()
    {
        if (waveClearedPanel != null)
            waveClearedPanel.SetActive(false);
    }

    public void ShowRestPeriod()
    {
        HideWaveClearedText();
        if (restPanel != null)
            restPanel.SetActive(true);
    }

    public void HideRestPeriod()
    {
        if (restPanel != null)
            restPanel.SetActive(false);
    }

    public IEnumerator ShowCountdown(float duration, string label)
    {
        float timeRemaining = duration;

        while (timeRemaining > 0)
        {
            if (restTimerText != null)
                restTimerText.text = label;

            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(timeRemaining).ToString();

            timeRemaining -= Time.deltaTime;
            yield return null;
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void ShowLosePanel()
    {
        if (losePanel != null)
            losePanel.SetActive(true);
    }

    public void HideAllPanels()
    {
        if (wavePanel != null) wavePanel.SetActive(false);
        if (waveClearedPanel != null) waveClearedPanel.SetActive(false);
        if (restPanel != null) restPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }
}
