using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TowerInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI rateOfFireText;
    public Image towerImage;

    [Header("Buttons")]
    public GameObject upgradeButton;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButtonComponent;
    public Button sellButton;
    public Button closeButton;
    public TextMeshProUGUI sellValueText;

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 3f, 0);
    //public bool billboardToCamera = true;

    //private Camera mainCamera;
    private TowerBehavior currentTower;
    [SerializeField]TowerSelectionManager towerSelectionManager;
    void Start()
    {
        // mainCamera = Camera.main;
     
        if (upgradeButtonComponent != null)
        {
            upgradeButtonComponent.onClick.AddListener(OnUpgradeClicked);
        }

        if (sellButton != null)
        {
            sellButton.onClick.AddListener(OnSellClicked);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(()=> towerSelectionManager.DeselectTower());
        }
        Hide();
    }

  
    public void ShowForTower(TowerBehavior tower)
    {
        currentTower = tower;
        gameObject.SetActive(true);
        towerImage.sprite = tower.towerData.towerIcon;  
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
            levelText.text = $"LVL {currentTower.upgradeLevel + 1}";

        UpdateStatsDisplay();
        UpdateUpgradeButton();
        UpdateSellButton();
    }

    void UpdateStatsDisplay()
    {
        bool canUpgrade = currentTower.CanUpgrade();

        if (canUpgrade)
        {
            float nextDamage = currentTower.towerData.baseDamage * Mathf.Pow(currentTower.towerData.damagePerLevel, currentTower.upgradeLevel + 1);
            float nextRange = currentTower.towerData.baseRange * Mathf.Pow(currentTower.towerData.rangePerLevel, currentTower.upgradeLevel + 1);
            float nextRateOfFire = currentTower.towerData.baseRateOfFire / Mathf.Pow(currentTower.towerData.rateOfFirePerLevel, currentTower.upgradeLevel + 1);

            if (damageText != null)
                damageText.text = $"Damage: {currentTower.damage:F1} -> <color=green>{nextDamage:F1}</color>";

            if (rangeText != null)
                rangeText.text = $"Range: {currentTower.range:F1} -> <color=green>{nextRange:F1}</color>";

            if (rateOfFireText != null)
                rateOfFireText.text = $"Rate: {(1f / currentTower.rateOfFire):F2}/s -> <color=green>{(1f / nextRateOfFire):F2}/s</color>";
        }
        else
        {
            if (damageText != null)
                damageText.text = $"Damage: {currentTower.damage:F1} <color=yellow>(MAX)</color>";

            if (rangeText != null)
                rangeText.text = $"Range: {currentTower.range:F1} <color=yellow>(MAX)</color>";

            if (rateOfFireText != null)
                rateOfFireText.text = $"Rate: {(1f / currentTower.rateOfFire):F2}/s <color=yellow>(MAX)</color>";
        }
    }

    void UpdateUpgradeButton()
    {
        bool canUpgrade = currentTower.CanUpgrade();

        if (upgradeButton != null)
        {
            upgradeButton.SetActive(canUpgrade);
        }

        if (!canUpgrade) return;

        int upgradeCost = currentTower.GetUpgradeCost();

        if (upgradeCostText != null)
        {
            upgradeCostText.text = $"Upgrade (${upgradeCost})";
        }

        if (upgradeButtonComponent != null)
        {
            bool canAfford = GameManager.instance != null && GameManager.instance.money >= upgradeCost;
            upgradeButtonComponent.interactable = canAfford;
        }
    }

    void UpdateSellButton()
    {
        int sellValue = currentTower.GetSellValue();

        if (sellValueText != null)
        {
            sellValueText.text = $"Sell (${sellValue})";
        }
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
        towerSelectionManager.DeselectTower();
    }
}
