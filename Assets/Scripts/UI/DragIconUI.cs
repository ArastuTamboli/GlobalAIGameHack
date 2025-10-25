using UnityEngine;
using UnityEngine.UI;

public class DragIconUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconImage;
    public RectTransform rectTransform;

    private Canvas canvas;

    void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();
    }

    public void SetIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.color = Color.white;
            }
        }
    }

    public void UpdatePosition(Vector2 screenPosition)
    {
        if (rectTransform == null || canvas == null) return;

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.worldCamera,
            out localPoint
        );

        rectTransform.localPosition = localPoint;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
