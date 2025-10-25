using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider healthSlider;
    public Image fillImage;

    [Header("Colors")]
    public Color highHealthColor = Color.green;
    public Color mediumHealthColor = Color.yellow;
    public Color lowHealthColor = Color.red;

  //  [Header("Settings")]

   // public bool billboardToCamera = true;

   // private Camera mainCamera;

    //void Start()
    //{
    //    mainCamera = Camera.main;
    //}

    //void LateUpdate()
    //{
    //    if (billboardToCamera && mainCamera != null)
    //    {
    //        transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
    //    }
    //}

    public void Initialize()
    {
     
        if (healthSlider != null)
        {
            healthSlider.value = 1f;
        }

        UpdateColor(1f);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider == null) return;

        float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
        healthSlider.value = healthPercent;

        UpdateColor(healthPercent);
    }

    void UpdateColor(float healthPercent)
    {
        if (fillImage == null) return;

        if (healthPercent > 0.6f)
        {
            fillImage.color = highHealthColor;
        }
        else if (healthPercent > 0.3f)
        {
            fillImage.color = mediumHealthColor;
        }
        else
        {
            fillImage.color = lowHealthColor;
        }
    }
}
