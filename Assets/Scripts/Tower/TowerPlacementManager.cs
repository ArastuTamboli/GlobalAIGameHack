using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager instance;
    [Header("Placement Settings")]
    public LayerMask towerPlacableLayer;
    public LayerMask towerLayer;
    public float placementHeight = 0f;
    public float minDistanceBetweenTowers = 2f;

    [Header("Visual Prefabs")]
    public GameObject rangeIndicatorPrefab;
    [Header("Upgrade Indicator")]
    public GameObject upgradeArrowPrefab;
    [Header("UI References")]
    public DragIconUI dragIcon;
    public Canvas canvas;

    [Header("References")]
    public Camera mainCamera;
    public GameManager gameManager;
    public CameraController cameraController;

    private TowerData selectedTowerData;
    private GameObject visualTower;
    private TowerVisual towerVisual;
    private bool isDragging = false;
    private bool isOverPlacableArea = false;
    private Vector3 currentPlacementPosition;

    private List<TowerBehavior> placedTowers = new List<TowerBehavior>();
    private TowerBehavior currentIndicatedTower = null;
    private GameObject activeUpgradeArrow = null;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (gameManager == null)
            gameManager = GameManager.instance;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (dragIcon != null)
            dragIcon.Hide();

        if (cameraController == null)
            cameraController = FindAnyObjectByType<CameraController>();

        if (gameManager != null)
        {
            gameManager.onCurrencyChanged.AddListener(OnMoneyChanged);
        }
    }

    void Update()
    {
        if (isDragging)
        {
            UpdateDragPosition();
        }
    }

    public bool CanAffordTower(TowerData towerData)
    {
        if (gameManager == null || towerData == null) return false;
        return gameManager.money >= towerData.baseCost;
    }

    public void StartPlacingTower(TowerData towerData)
    {
        if (isDragging) return;

        if (!CanAffordTower(towerData))
        {
            Debug.Log("Not enough money!");
            return;
        }

        selectedTowerData = towerData;
        isDragging = true;
        DisableCameraInput();
        if (dragIcon != null)
        {
            dragIcon.SetIcon(towerData.towerIcon);
            dragIcon.Show();
        }
    }

    void UpdateDragPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (dragIcon != null)
        {
            dragIcon.UpdatePosition(mousePosition);
        }

        CheckPlacableArea(mousePosition);
    }

    void CheckPlacableArea(Vector2 screenPosition)
    {
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
        //    DestroyVisualTower();
        //    isOverPlacableArea = false;
        //    return;
        //}

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerPlacableLayer))
        {
            Debug.Log("Hit placable area at " + hit.point);    
            isOverPlacableArea = true;
            currentPlacementPosition = hit.point;
            currentPlacementPosition.y = placementHeight;

            if (visualTower == null)
            {
                CreateVisualTower();
            }

            if (visualTower != null)
            {
                visualTower.transform.position = currentPlacementPosition;
                UpdatePlacementValidity();
            }

            if (dragIcon != null)
            {
                dragIcon.Hide();
            }
        }
        else
        {
            DestroyVisualTower();
            isOverPlacableArea = false;

            if (dragIcon != null)
            {
                dragIcon.Show();
            }
        }
    }

    void CreateVisualTower()
    {
        if (selectedTowerData == null || selectedTowerData.prefab == null) return;
        Debug.Log("Create Visual Tower ");
        visualTower = Instantiate(selectedTowerData.prefab, currentPlacementPosition, Quaternion.identity);
        visualTower.name = "Visual_" + selectedTowerData.towerName;

        TowerBehavior towerBehavior = visualTower.GetComponent<TowerBehavior>();
        if (towerBehavior != null)
        {
            Destroy(towerBehavior);
        }

        Collider[] colliders = visualTower.GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            Destroy(col);
        }

        towerVisual = visualTower.AddComponent<TowerVisual>();
      
        CreateRangeIndicator();
    }

    void CreateRangeIndicator()
    {
        if (visualTower == null || selectedTowerData == null) return;

        GameObject rangeIndicator;

        
            rangeIndicator = Instantiate(rangeIndicatorPrefab, visualTower.transform);
            rangeIndicator.transform.localPosition = Vector3.zero;
            rangeIndicator.transform.localRotation = Quaternion.identity;
        
       

        float diameter = selectedTowerData.baseRange * 2f;
        rangeIndicator.transform.localScale = new Vector3(diameter, 0.01f, diameter);

        if (towerVisual != null)
        {
            towerVisual.rangeIndicator = rangeIndicator;
            towerVisual.SetRangeIndicatorSize(selectedTowerData.baseRange);
        }
    }


    void UpdatePlacementValidity()
    {
        if (visualTower == null || towerVisual == null) return;

        bool valid = true;

        Collider[] nearbyTowers = Physics.OverlapSphere(
            currentPlacementPosition,
            minDistanceBetweenTowers,
            towerLayer
        );

        if (nearbyTowers.Length > 0)
        {
            valid = false;
        }

        towerVisual.SetValidPlacement(valid);
    }

    void DestroyVisualTower()
    {
        if (visualTower != null)
        {
            Destroy(visualTower);
            visualTower = null;
            towerVisual = null;
        }
    }

    public void TryPlaceTower()
    {
        Debug.Log("Ended Dragging Tower Slot 5");
        if (!isDragging)
        {
            return;
        }
        Debug.Log("Ended Dragging Tower Slot 6 " + visualTower + towerVisual + isOverPlacableArea);
        if (!isOverPlacableArea || visualTower == null || towerVisual == null)
        {
            CancelPlacement();
            return;
        }
        Debug.Log("Ended Dragging Tower Slot 7");
        Collider[] nearbyTowers = Physics.OverlapSphere(
            currentPlacementPosition,
            minDistanceBetweenTowers,
            towerLayer
        );

        if (nearbyTowers.Length > 0)
        {
            Debug.Log("Cannot place tower - too close to another tower!");
            CancelPlacement();
            return;
        }

        if (gameManager != null && !gameManager.SpendMoney(selectedTowerData.baseCost))
        {
            Debug.Log("Not enough money!");
            CancelPlacement();
            return;
        }

        PlaceTower();
    }

    void PlaceTower()
    {
        GameObject newTower = Instantiate(
            selectedTowerData.prefab,
            currentPlacementPosition,
            Quaternion.identity
        );

        newTower.name = selectedTowerData.towerName;
        newTower.layer = LayerMask.NameToLayer("Tower");
        TowerBehavior towerBehavior = newTower.GetComponent<TowerBehavior>();
        if (towerBehavior != null)
        {
            towerBehavior.towerData = selectedTowerData;
            RegisterTower(towerBehavior);
        }

        Debug.Log($"Placed {selectedTowerData.towerName} at {currentPlacementPosition}");

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        DestroyVisualTower();

        if (dragIcon != null)
        {
            dragIcon.Hide();
        }

        EnableCameraInput();

        isDragging = false;
        isOverPlacableArea = false;
        selectedTowerData = null;
    }

    void DisableCameraInput()
    {
        if (cameraController != null)
        {
            cameraController.SetInputEnabled(false);
        }
    }

    void EnableCameraInput()
    {
        if (cameraController != null)
        {
            cameraController.SetInputEnabled(true);
        }
    }

    public bool IsDragging()
    {
        return isDragging;
    }
    public void RegisterTower(TowerBehavior tower)
    {
        if (tower != null && !placedTowers.Contains(tower))
        {
            placedTowers.Add(tower);
            Debug.Log($"Tower registered: {tower.name}. Total towers: {placedTowers.Count}");
        }
    }

    public void UnregisterTower(TowerBehavior tower)
    {
        if (tower != null && placedTowers.Contains(tower))
        {
            placedTowers.Remove(tower);
            Debug.Log($"Tower unregistered: {tower.name}. Total towers: {placedTowers.Count}");

            if (currentIndicatedTower == tower)
            {
                HideUpgradeArrow();
            }
        }
    }

    public List<TowerBehavior> GetAllTowers()
    {
        placedTowers.RemoveAll(tower => tower == null);
        return placedTowers;
    }

    void OnMoneyChanged(int newMoney)
    {
        CheckAndShowUpgradeIndicator(newMoney);
    }

    void CheckAndShowUpgradeIndicator(int currentMoney)
    {
        if (activeUpgradeArrow != null)
        {
            return;
        }

        List<TowerBehavior> upgradableTowers = GetUpgradableTowers(currentMoney);

        if (upgradableTowers.Count > 0)
        {
            TowerBehavior randomTower = upgradableTowers[Random.Range(0, upgradableTowers.Count)];
            ShowUpgradeArrowOnTower(randomTower);
        }
    }

    List<TowerBehavior> GetUpgradableTowers(int currentMoney)
    {
        List<TowerBehavior> upgradable = new List<TowerBehavior>();

        placedTowers.RemoveAll(tower => tower == null);

        foreach (TowerBehavior tower in placedTowers)
        {
            if (tower.CanUpgrade() && tower.GetUpgradeCost() <= currentMoney)
            {
                upgradable.Add(tower);
            }
        }

        return upgradable;
    }

    void ShowUpgradeArrowOnTower(TowerBehavior tower)
    {
        if (upgradeArrowPrefab == null)
        {
            Debug.LogWarning("Upgrade arrow prefab not assigned!");
            return;
        }

        HideUpgradeArrow();

        activeUpgradeArrow = Instantiate(upgradeArrowPrefab, tower.transform);

        Vector3 arrowPosition = tower.transform.position + Vector3.up * 3f;
        activeUpgradeArrow.transform.position = arrowPosition;

        currentIndicatedTower = tower;

        Debug.Log($"<color=yellow> Upgrade available for {tower.towerData.towerName}!</color>");
    }

    public void HideUpgradeArrow()
    {
        if (activeUpgradeArrow != null)
        {
            Destroy(activeUpgradeArrow);
            activeUpgradeArrow = null;
            currentIndicatedTower = null;
        }
    }

    public void OnTowerUpgraded(TowerBehavior tower)
    {
        if (currentIndicatedTower == tower)
        {
            HideUpgradeArrow();
        }
    }

    public void OnTowerSold(TowerBehavior tower)
    {
        UnregisterTower(tower);
    }
    void OnDestroy()
    {
        EnableCameraInput();
        if (gameManager != null)
        {
            gameManager.onCurrencyChanged.RemoveListener(OnMoneyChanged);
        }
    }
}
