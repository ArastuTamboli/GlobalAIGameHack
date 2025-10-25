using UnityEngine;

public class TowerVisual : MonoBehaviour
{
    [Header("Visual Settings")]
    public GameObject rangeIndicator;

    private bool isValidPlacement = true;
    private Material[] originalMaterials;

    void Start()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.SetActive(true);
        }

    }

    public void SetValidPlacement(bool valid)
    {
        isValidPlacement = valid;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        Color color = isValidPlacement ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);

        if (rangeIndicator != null)
        {
            Renderer rangeRenderer = rangeIndicator.GetComponent<Renderer>();
            if (rangeRenderer != null)
            {
                rangeRenderer.material.color = color;
            }
        }
    }

    public void SetRangeIndicatorSize(float range)
    {
        if (rangeIndicator != null)
        {
            float diameter = range * 2f;
            rangeIndicator.transform.localScale = new Vector3(diameter, 0.01f, diameter);
        }
    }
}
