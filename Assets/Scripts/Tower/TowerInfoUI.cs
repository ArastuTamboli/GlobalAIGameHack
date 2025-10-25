using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI towerNameText;
    public Image towerImage;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI rateOfFireText;

    [Header("Upgrade Info")]
    public GameObject upgradePanel;
    public TextMeshProUGUI nextDamageText;
    public TextMeshProUGUI nextRangeText;
    public TextMeshProUGUI nextRateOfFireText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;

    [Header("Sell Info")]
    public TextMeshProUGUI sellValueText;
    public Button sellButton;


    private TowerBehavior currentTower;

    void Start()
    {
        

        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(OnSellClicked);
        }

        Hide();
    }

    public void ShowForTower(TowerBehavior tower)
    {
        currentTower = tower;
        gameObject.SetActive(true);

        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentTower = null;
    }

    void UpdateUI()
    {
        if (currentTower == null || currentTower.towerData == null) return;

        if (towerNameText != null)
            towerNameText.text = currentTower.towerData.towerName;

        if (levelText != null)
            levelText.text = $"Level {currentTower.upgradeLevel + 1}";

        if (damageText != null)
            damageText.text = $"Damage: {currentTower.damage:F1}";

        if (rangeText != null)
            rangeText.text = $"Range: {currentTower.range:F1}";

        if (rateOfFireText != null)
            rateOfFireText.text = $"Rate: {(1f / currentTower.rateOfFire):F2}/s";

        UpdateUpgradeUI();
        UpdateSellUI();
    }

    void UpdateUpgradeUI()
    {
        bool canUpgrade = currentTower.CanUpgrade();

        if (upgradePanel != null)
            upgradePanel.SetActive(canUpgrade);

        if (!canUpgrade) return;

        int upgradeCost = currentTower.GetUpgradeCost();
        float nextDamage = currentTower.towerData.baseDamage * Mathf.Pow(currentTower.towerData.damagePerLevel, currentTower.upgradeLevel + 1);
        float nextRange = currentTower.towerData.baseRange * Mathf.Pow(currentTower.towerData.rangePerLevel, currentTower.upgradeLevel + 1);
        float nextRateOfFire = currentTower.towerData.baseRateOfFire / Mathf.Pow(currentTower.towerData.rateOfFirePerLevel, currentTower.upgradeLevel + 1);

        if (nextDamageText != null)
            nextDamageText.text = $"-> {nextDamage:F1}";

        if (nextRangeText != null)
            nextRangeText.text = $"-> {nextRange:F1}";

        if (nextRateOfFireText != null)
            nextRateOfFireText.text = $"-> {(1f / nextRateOfFire):F2}/s";

        if (upgradeCostText != null)
            upgradeCostText.text = $"${upgradeCost}";

        if (upgradeButton != null)
        {
            bool canAfford = GameManager.instance != null && GameManager.instance.money >= upgradeCost;
            upgradeButton.interactable = canAfford;
        }
    }

    void UpdateSellUI()
    {
        int sellValue = currentTower.GetSellValue();

        if (sellValueText != null)
            sellValueText.text = $"Sell: ${sellValue}";
    }

    void OnUpgradeClicked()
    {
        if (currentTower == null) return;

        int upgradeCost = currentTower.GetUpgradeCost();

        if (GameManager.instance != null && GameManager.instance.SpendMoney(upgradeCost))
        {
            currentTower.Upgrade();
            UpdateUI();
        }
    }

    void OnSellClicked()
    {
        if (currentTower == null) return;

        currentTower.Sell();
        Hide();
    }
}
