using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower Defense/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Tower Info")]
    public string towerName;
    public GameObject prefab;

    [Header("Unlock")]
    public int unlockAtWave = 1;
    [Header("Base Stats")]
    public float baseDamage = 100f;
    public float baseRateOfFire = 1f;
    public float baseRange = 10f;

    [Header("Effectiveness")]
    public EnemyType[] strongAgainst;
    public EnemyType[] weakAgainst;

    [Header("Costs")]
    public int baseCost = 100;
    public float upgradeCostMultiplier = 1.4f;

    [Header("Upgrade Scaling")]
    public float damagePerLevel = 1.2f;
    public float rangePerLevel = 1.15f;
    public float rateOfFirePerLevel = 1.1f;

    [Header("Visual")]
    public Sprite towerIcon;

    public int GetUpgradeCost(int currentLevel)
    {
        if (currentLevel >= 3) return 0;
        return Mathf.RoundToInt(baseCost * Mathf.Pow(upgradeCostMultiplier, currentLevel));
    }

    public bool IsUnlockedAtWave(int currentWave)
    {
        return currentWave >= unlockAtWave;
    }
}
