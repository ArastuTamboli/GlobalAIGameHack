using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamagePostProcessing : MonoBehaviour
{
    public static DamagePostProcessing Instance;

    [Header("References")]
    public Volume postProcessVolume;

    [Header("Chromatic Aberration Settings")]
    public float chromaticAberrationPeak = 0.8f;
    public float chromaticAberrationDuration = 0.5f;

    [Header("Bloom Settings")]
    public float bloomPeakIntensity = 3f;
    public float bloomDuration = 0.5f;

    [Header("Vignette Settings")]
    public float vignettePeakIntensity = 0.5f;
    public float vignetteDuration = 0.5f;
    public Color vignetteColor = Color.red;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip criticalDamageSound;
    public AudioClip emergencyDamageSound;

    private ChromaticAberration chromaticAberration;
    private Bloom bloom;
    private Vignette vignette;

    private float chromaticAberrationDefault = 0f;
    private float bloomDefault = 1f;
    private float vignetteDefault = 0f;

    private Coroutine chromaticCoroutine;
    private Coroutine bloomCoroutine;
    private Coroutine vignetteCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out bloom);
            postProcessVolume.profile.TryGet(out vignette);

            if (chromaticAberration != null)
            {
                chromaticAberrationDefault = chromaticAberration.intensity.value;
            }

            if (bloom != null)
            {
                bloomDefault = bloom.intensity.value;
            }

            if (vignette != null)
            {
                vignetteDefault = vignette.intensity.value;
            }
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.onAnyDamage.AddListener(OnAnyGateDamage);
         
        }
    }

    void OnAnyGateDamage()
    {
        TriggerDamageEffect(1f);
        PlaySound(damageSound);
    }

   

    public void TriggerDamageEffect(float intensityMultiplier = 1f)
    {
        if (chromaticCoroutine != null) StopCoroutine(chromaticCoroutine);
        if (bloomCoroutine != null) StopCoroutine(bloomCoroutine);
        if (vignetteCoroutine != null) StopCoroutine(vignetteCoroutine);

        if (chromaticAberration != null)
        {
            chromaticCoroutine = StartCoroutine(AnimateChromaticAberration(intensityMultiplier));
        }

        if (bloom != null)
        {
            bloomCoroutine = StartCoroutine(AnimateBloom(intensityMultiplier));
        }

        if (vignette != null)
        {
            vignetteCoroutine = StartCoroutine(AnimateVignette(intensityMultiplier));
        }
    }

    IEnumerator AnimateChromaticAberration(float intensityMultiplier)
    {
        float peakValue = chromaticAberrationPeak * intensityMultiplier;
        chromaticAberration.intensity.value = peakValue;

        float elapsed = 0f;

        while (elapsed < chromaticAberrationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / chromaticAberrationDuration;

            chromaticAberration.intensity.value = Mathf.Lerp(peakValue, chromaticAberrationDefault, t);

            yield return null;
        }

        chromaticAberration.intensity.value = chromaticAberrationDefault;
        chromaticCoroutine = null;
    }

    IEnumerator AnimateBloom(float intensityMultiplier)
    {
        float peakValue = bloomPeakIntensity * intensityMultiplier;
        bloom.intensity.value = peakValue;

        float elapsed = 0f;

        while (elapsed < bloomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bloomDuration;

            bloom.intensity.value = Mathf.Lerp(peakValue, bloomDefault, t);

            yield return null;
        }

        bloom.intensity.value = bloomDefault;
        bloomCoroutine = null;
    }

    IEnumerator AnimateVignette(float intensityMultiplier)
    {
        float peakValue = vignettePeakIntensity * intensityMultiplier;
        vignette.intensity.value = peakValue;
        vignette.color.value = vignetteColor;

        float elapsed = 0f;

        while (elapsed < vignetteDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / vignetteDuration;

            vignette.intensity.value = Mathf.Lerp(peakValue, vignetteDefault, t);

            yield return null;
        }

        vignette.intensity.value = vignetteDefault;
        vignetteCoroutine = null;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
