using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.1f;
    public bool playOnAwake = true;
    public bool loopMusic = true;

    [Header("Fade Settings")]
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 1f;
    public float crossfadeDuration = 1.5f;

    private AudioSource audioSource;
    private AudioSource crossfadeSource;
    private Coroutine currentFadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    void Start()
    {
        if (playOnAwake && menuMusic != null)
        {
            PlayMusic(menuMusic, fadeInDuration);
        }
    }

    void SetupAudioSources()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = loopMusic;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;

        crossfadeSource = gameObject.AddComponent<AudioSource>();
        crossfadeSource.loop = loopMusic;
        crossfadeSource.playOnAwake = false;
        crossfadeSource.volume = 0f;
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 0f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        if (fadeDuration > 0f)
        {
            currentFadeCoroutine = StartCoroutine(FadeInMusic(clip, fadeDuration));
        }
        else
        {
            audioSource.clip = clip;
            audioSource.volume = musicVolume;
            audioSource.Play();
        }
    }

    public void CrossfadeToMusic(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("AudioClip is null!");
            return;
        }

        if (audioSource.clip == newClip && audioSource.isPlaying)
        {
            return;
        }

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(CrossfadeMusic(newClip));
    }

    public void StopMusic(float fadeDuration = 0f)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        if (fadeDuration > 0f)
        {
            currentFadeCoroutine = StartCoroutine(FadeOutMusic(fadeDuration));
        }
        else
        {
            audioSource.Stop();
            audioSource.volume = 0f;
        }
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public void PlayMenuMusic()
    {
        CrossfadeToMusic(menuMusic);
    }

    public void PlayGameplayMusic()
    {
        CrossfadeToMusic(gameplayMusic);
    }

    IEnumerator FadeInMusic(AudioClip clip, float duration)
    {
        audioSource.clip = clip;
        audioSource.volume = 0f;
        audioSource.Play();

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = musicVolume;
        currentFadeCoroutine = null;
    }

    IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        currentFadeCoroutine = null;
    }

    IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        AudioSource oldSource = audioSource;
        AudioSource newSource = crossfadeSource;

        newSource.clip = newClip;
        newSource.volume = 0f;
        newSource.Play();

        float elapsed = 0f;
        float oldStartVolume = oldSource.volume;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeDuration;

            oldSource.volume = Mathf.Lerp(oldStartVolume, 0f, t);
            newSource.volume = Mathf.Lerp(0f, musicVolume, t);

            yield return null;
        }

        oldSource.volume = 0f;
        oldSource.Stop();
        newSource.volume = musicVolume;

        audioSource = newSource;
        crossfadeSource = oldSource;

        currentFadeCoroutine = null;
    }
}
