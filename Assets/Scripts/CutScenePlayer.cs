using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public string nextSceneName = "GameScene";
    
    [Header("UI")]
    public GameObject skipButton;
    public CanvasGroup fadePanel;
    
    [Header("Loading Screen")]
    public GameObject loadingPanel;
    public Image loadingImage;
    public TextMeshProUGUI loadingText;
    public bool animateLoadingImage = true;
    public float rotationSpeed = 100f;
    
    [Header("Settings")]
    public bool allowSkip = true;
    public float fadeOutDuration = 1f;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (skipButton != null)
            skipButton.SetActive(allowSkip);

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }

        if (fadePanel != null)
            fadePanel.alpha = 0f;

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    void Update()
    {
        if (allowSkip && Input.GetKeyDown(KeyCode.Escape))
        {
            SkipCutscene();
        }
    }

    public void SkipCutscene()
    {
        Debug.Log("[CUTSCENE]: Skipped");
        LoadNextScene();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("[CUTSCENE]: Video ended");
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            
            if (animateLoadingImage && loadingImage != null)
            {
                StartCoroutine(SpinLoadingImage());
            }
        }

        if (fadePanel != null)
        {
            StartCoroutine(FadeAndLoad());
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    System.Collections.IEnumerator FadeAndLoad()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    System.Collections.IEnumerator SpinLoadingImage()
    {
        while (loadingPanel.activeSelf)
        {
            loadingImage.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}