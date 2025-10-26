using Neocortex;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VesperManager : MonoBehaviour
{
    public static VesperManager Instance;

    [Header("References")]
    public WaveManager waveManager;
    public GameManager gameManager;
    public PathProgressTrigger[] path1Triggers;
    public PathProgressTrigger[] path2Triggers;

    [Header("UI Display")]
    public TextMeshProUGUI vesperResponseText;
    public TextMeshProUGUI gameStateText;

  

    private Dictionary<string, int> path1Breaches = new Dictionary<string, int>();
    private Dictionary<string, int> path2Breaches = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //if (waveManager != null)
        //{
        //    waveManager.onWaveCleared.AddListener(OnWaveCleared);
        //}
        if (GameManager.instance != null)
        {
            GameManager.instance.onFirstDamage.AddListener(OnFirstDamage);
            GameManager.instance.onHealthThreshold50.AddListener(OnHealthCritical50);
            GameManager.instance.onHealthThreshold20.AddListener(OnHealthCritical20);
        }
        InvokeRepeating(nameof(UpdateGameStateDisplay), 1f, 2f);
    }

    void OnFirstDamage()
    {
        Debug.Log("<color=yellow>[VESPER ALERT]</color> First damage detected!");

        string prompt = "[ALERT] Gate has taken first damage!\n";
        prompt += "Enemies are breaking through defenses.\n";
        prompt += GetCurrentGameState();
        prompt += "\nProvide immediate tactical response.";
        VoiceCommandController.instance.OnDamageToNeocortex(prompt);

    }
    void OnHealthCritical50(float healthPercentage)
    {
        Debug.Log("<color=orange>[VESPER ALERT]</color> Gate at 50% health!");

        string prompt = $"[CRITICAL] Gate health at {healthPercentage:P0}!\n";
        prompt += "Defense line is failing. Multiple breaches detected.\n";
        prompt += GetCurrentGameState();
        prompt += "\nProvide emergency defense strategy.";

        VoiceCommandController.instance.OnDamageToNeocortex(prompt);
    }
    void OnHealthCritical20(float healthPercentage)
    {
        Debug.Log("<color=red>[VESPER ALERT]</color> Gate at 20% health!");

        string prompt = $"[EMERGENCY] Gate health CRITICAL at {healthPercentage:P0}!\n";
        prompt += "Imminent failure. Last stand protocols needed.\n";
        prompt += GetCurrentGameState();
        prompt += "\nProvide last-resort survival strategy.";
        VoiceCommandController.instance.OnDamageToNeocortex(prompt);

    }
    void OnWaveCleared(int waveNumber)
    {
        Debug.Log($"<color=yellow>[VESPER]</color> Wave {waveNumber} cleared. Analyzing performance...");
        RequestPostWaveAnalysis();
   
    }
    public void OnClickAnalysisButton()
    {
       
        RequestPostWaveAnalysis();

    }
    void RequestPostWaveAnalysis()
    {
        ClearBreachData();
        string analysis = GetPostWaveAnalysisPrompt();
        VoiceCommandController.instance.PostRequest(analysis);
    }

    void OnPostWaveResponse(string response)
    {
        Debug.Log($"<color=green>[VESPER POST-WAVE ANALYSIS]</color>\n{response}");

        if (vesperResponseText != null)
        {
            vesperResponseText.text = $"VESPER ANALYSIS:\n{response}";
        }

        ClearBreachData();
    }

    string GetPostWaveAnalysisPrompt()
    {
        int wave = waveManager != null ? waveManager.currentWaveIndex + 1 : 1;
        int currency = gameManager != null ? gameManager.money : 0;

        float path1MaxBreach = GetPathBreachPercentage(path1Triggers);
        float path2MaxBreach = GetPathBreachPercentage(path2Triggers);

        string towerInfo = GetAllTowersInfo();

        string prompt = $"[POST-WAVE ANALYSIS] Wave {wave} Complete\n\n";
        prompt += $"[RESOURCES] Currency: {currency} EC\n\n";
        prompt += $"[PERFORMANCE]\n";
        prompt += $"- Left Path Max Breach: {path1MaxBreach:P0}\n";
        prompt += $"- Right Path Max Breach: {path2MaxBreach:P0}\n\n";
        prompt += $"[CURRENT TOWERS]\n{towerInfo}\n\n";
        prompt += $"Provide analysis:\n";
        prompt += $"1. Which path had issues?\n";
        prompt += $"2. Which towers should I upgrade? (if affordable)\n";
        prompt += $"3. Which towers should I sell? (if ineffective)\n";
        prompt += $"4. Where should I build next?\n\n";
        prompt += $"Keep response SHORT and actionable.";

        return prompt;
    }

    string GetAllTowersInfo2()
    {
        TowerBehavior[] towers = FindObjectsOfType<TowerBehavior>();

        if (towers.Length == 0)
        {
            return "No towers placed yet.";
        }

        string info = "";
        for (int i = 0; i < towers.Length; i++)
        {
            TowerBehavior tower = towers[i];
            string towerName = tower.towerData != null ? tower.towerData.towerName : "Unknown";
            int level = tower.upgradeLevel;
            Vector3 pos = tower.transform.position;

            string zone = DetermineZone(pos);

            info += $"- {towerName} Lv{level} at {zone}\n";
        }

        return info;
    }
    string GetAllTowersInfo()
    {
        if (TowerPlacementManager.instance == null)
            return "No tower manager found.";

        List<TowerBehavior> towers = TowerPlacementManager.instance.GetAllTowers();

        if (towers.Count == 0)
        {
            return "No towers placed yet.";
        }

        string info = "";
        for (int i = 0; i < towers.Count; i++)
        {
            TowerBehavior tower = towers[i];
            string towerName = tower.towerData != null ? tower.towerData.towerName : "Unknown";
            int level = tower.upgradeLevel;
            Vector3 pos = tower.transform.position;

            string zone = DetermineZone(pos);

            info += $"- {towerName} Lv{level} at {zone}\n";
        }

        return info;
    }
    string DetermineZone(Vector3 position)
    {
        if (position.x < -1f) return "Orange Path";
        if (position.x > 1f) return "Blue Path";
        return "Middle";
    }

    public void RequestAIAnalysis()
    {
        string gameState = GetCurrentGameState();

        Debug.Log($"<color=cyan>[VESPER REQUEST]</color>\n{gameState}");
        VoiceCommandController.instance.PostRequest(gameState);
        //if (neocortexAgent != null)
        //{
        //    neocortexAgent.SendMessage(gameState, OnAIResponse);
        //}
        //else
        //{
        //    Debug.LogWarning("Neocortex Agent not assigned!");
        //}
    }

    void OnAIResponse(string response)
    {
        Debug.Log($"<color=green>[VESPER RESPONSE]</color>\n{response}");

        if (vesperResponseText != null)
        {
            vesperResponseText.text = $"VESPER: {response}";
        }
    }

    string GetCurrentGameState()
    {
        int wave = waveManager != null ? waveManager.currentWaveIndex + 1 : 1;
        int currency = gameManager != null ? gameManager.money : 0;
        int activeEnemies = waveManager != null ? waveManager.GetActiveEnemyCount() : 0;
        int totalEnemies = waveManager != null ? waveManager.GetCurrentWaveTotalEnemies() : 0;

        float path1Breach = GetPathBreachPercentage(path1Triggers);
        float path2Breach = GetPathBreachPercentage(path2Triggers);

        string state = $"[STATE] Wave {wave}, Currency {currency} EC, Enemies {activeEnemies}/{totalEnemies}.\n";
        state += $"[THREAT] Left Path: {path1Breach:P0} breach. Right Path: {path2Breach:P0} breach.\n";

        if (path1Breach >= 0.5f || path2Breach >= 0.5f)
        {
            state += "[STATUS] CRITICAL - Enemies breaking through!\n";
        }

        state += "Analyze situation and recommend action.";

        return state;
    }

    float GetPathBreachPercentage(PathProgressTrigger[] triggers)
    {
        if (triggers == null || triggers.Length == 0) return 0f;

        float maxPercentage = 0f;

        foreach (var trigger in triggers)
        {
            if (trigger != null)
            {
                float percentage = trigger.GetCurrentPercentage();
                if (percentage > maxPercentage)
                    maxPercentage = percentage;
            }
        }

        return maxPercentage;
    }

    void UpdateGameStateDisplay()
    {
        if (gameStateText == null) return;

        int wave = waveManager != null ? waveManager.currentWaveIndex + 1 : 1;
        int currency = gameManager != null ? gameManager.money : 0;
        int activeEnemies = waveManager != null ? waveManager.GetActiveEnemyCount() : 0;
        float path1Breach = GetPathBreachPercentage(path1Triggers);
        float path2Breach = GetPathBreachPercentage(path2Triggers);

        gameStateText.text = $"WAVE {wave} | EC: {currency} | ENEMIES: {activeEnemies}\n" +
                            $"L-PATH: {path1Breach:P0} | R-PATH: {path2Breach:P0}";
    }

    public void OnTriggerBreach(string zone, int pathIndex, float percentage)
    {
        Debug.Log($"<color=yellow>[VESPER ALERT]</color> {zone} breach at {percentage:P0}!");

        if (pathIndex == 0)
            path1Breaches[zone] = Mathf.RoundToInt(percentage * 100);
        else
            path2Breaches[zone] = Mathf.RoundToInt(percentage * 100);
    }

    void ClearBreachData()
    {
        path1Breaches.Clear();
        path2Breaches.Clear();
    }
}
