using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSelectionManager : MonoBehaviour
{
    [Header("UI")]
    public TowerInfoUI towerInfoUI;

    [Header("Settings")]
    public LayerMask towerLayer;

    [Header("References")]
    public Camera mainCamera;

    private TowerBehavior selectedTower;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (towerInfoUI == null)
            towerInfoUI = FindObjectOfType<TowerInfoUI>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTowerClick();
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectTower();
        }
    }

    void HandleTowerClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerLayer))
        {
            TowerBehavior tower = hit.collider.GetComponent<TowerBehavior>();

            if (tower == null)
                tower = hit.collider.GetComponentInParent<TowerBehavior>();

            if (tower != null)
            {
                SelectTower(tower);
            }
        }
        else
        {
            DeselectTower();
        }
    }

    void SelectTower(TowerBehavior tower)
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        selectedTower = tower;
        selectedTower.ShowRange();

        if (towerInfoUI != null)
        {
            towerInfoUI.ShowForTower(tower);
        }
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange();
            selectedTower = null;
        }

        if (towerInfoUI != null)
        {
            towerInfoUI.Hide();
        }
    }
}
