using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TowerSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tower Data")]
    public TowerData towerData;
    public bool isUnlocked = true;

    [Header("UI Elements")]
    public Image towerIcon;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI unlockWaveText;
    public GameObject lockedOverlay;
    public GameObject insufficientMoneyOverlay;
    public Button selectButton;

    [Header("References")]
    public TowerPlacementManager placementManager;

    private bool isDragging = false;

    void Start()
    {
        UpdateUI();

    }

    void Update()
    {
        UpdateMoneyIndicator();
    }

    public void Initialize(TowerData data, bool unlocked,TowerPlacementManager _placementManager)
    {
        towerData = data;
        isUnlocked = unlocked;
        placementManager = _placementManager;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (towerData == null) return;

        if (towerNameText != null)
            towerNameText.text = towerData.towerName;

        if (costText != null)
            costText.text = $"${towerData.baseCost}";

        if (unlockWaveText != null)
        {
            if (isUnlocked)
            {
                unlockWaveText.gameObject.SetActive(false);
            }
            else
            {
                unlockWaveText.gameObject.SetActive(true);
                unlockWaveText.text = $"Unlocks Wave {towerData.unlockAtWave}";
            }
        }

        if (towerIcon != null)
        {
            if (towerData.towerIcon != null)
            {
                towerIcon.sprite = towerData.towerIcon;
                towerIcon.color = Color.white;
            }
        }

        if (lockedOverlay != null)
            lockedOverlay.SetActive(!isUnlocked);

        UpdateMoneyIndicator();
    }

    void UpdateMoneyIndicator()
    {
        if (insufficientMoneyOverlay == null || placementManager == null || towerData == null)
            return;

        bool canAfford = placementManager.CanAffordTower(towerData);
        insufficientMoneyOverlay.SetActive(!canAfford && isUnlocked);
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        UpdateUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isUnlocked || towerData == null) return;

        if (placementManager == null || !placementManager.CanAffordTower(towerData))
        {
            return;
        }

        isDragging = true;
        placementManager.StartPlacingTower(towerData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Ended Dragging Tower Slot 1");
        if (!isDragging) return;
        Debug.Log("Ended Dragging Tower Slot 2");
        isDragging = false;
        Debug.Log("Ended Dragging Tower Slot 3");
        if (placementManager != null)
        {
            placementManager.TryPlaceTower();
        }
    }
}
