using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.2f;
    public float shakeFrequency = 25f;

    private Vector3 originalPosition;
    private Coroutine currentShakeCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        originalPosition = transform.localPosition;

        if (GameManager.instance != null)
        {
            GameManager.instance.onAnyDamage.AddListener(OnGateDamaged);
        
        }
    }

    void OnGateDamaged()
    {
        TriggerShake(0.3f, 0.15f);
    }

   

    public void TriggerShake(float duration, float magnitude)
    {
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
        }

        currentShakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void TriggerShake()
    {
        TriggerShake(shakeDuration, shakeMagnitude);
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = 1f - (elapsed / duration);

            Vector3 shakeOffset = Random.insideUnitSphere * magnitude * intensity;
            transform.localPosition = originalPosition + shakeOffset;

            yield return null;
        }

        transform.localPosition = originalPosition;
        currentShakeCoroutine = null;
    }
}
