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

    void OnDestroy()
    {
        EnableCameraInput();
    }
}
