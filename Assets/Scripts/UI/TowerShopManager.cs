using UnityEngine;
using System.Collections.Generic;

public class TowerShopManager : MonoBehaviour
{
    [Header("Tower Data")]
    public List<TowerData> allTowers = new List<TowerData>();

  
    [Header("UI")]
    public Transform towerSlotContainer;
    public GameObject towerSlotPrefab;

    [Header("References")]
    TowerPlacementManager placementManager;
    WaveManager waveManager;

    private List<TowerSlotUI> towerSlots = new List<TowerSlotUI>();

    void Start()
    {
        if (placementManager == null)
            placementManager = TowerPlacementManager.instance;

        if (waveManager == null)
            waveManager = WaveManager.Instance;

        InitializeTowerShop();

        if (waveManager != null)
        {
            waveManager.onWaveStart.AddListener(OnWaveStart);
        }
    }

    void InitializeTowerShop()
    {
        foreach (TowerData towerData in allTowers)
        {
            if (towerData == null) continue;

            int currentWave = waveManager != null ? waveManager.currentWaveIndex + 1 : 1;
            bool isUnlocked = towerData.IsUnlockedAtWave(currentWave);

            CreateTowerSlot(towerData, isUnlocked);
        }
    }

    void CreateTowerSlot(TowerData towerData, bool isUnlocked)
    {
        GameObject slotObj = Instantiate(towerSlotPrefab, towerSlotContainer);
        TowerSlotUI slotUI = slotObj.GetComponent<TowerSlotUI>();

        if (slotUI != null)
        {
            slotUI.Initialize(towerData, isUnlocked, placementManager);
            towerSlots.Add(slotUI);
        }
    }

    void OnWaveStart(int waveNumber)
    {
        foreach (TowerSlotUI slot in towerSlots)
        {
            if (slot.towerData != null && slot.towerData.unlockAtWave == waveNumber)
            {
                slot.SetUnlocked(true);
                Debug.Log($"{slot.towerData.towerName} unlocked at wave {waveNumber}!");
            }
        }
    }

    public void ToggleShop()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
