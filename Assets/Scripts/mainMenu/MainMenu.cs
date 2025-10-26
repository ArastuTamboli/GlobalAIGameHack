using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string GAME_SCENE_NAME = "GameScene";
    private const string GAMEPLAY_BUTTON_SCREEN_NAME = "GameplayButtonScreen";
    
    public void StartGame()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }
    
    public void OpenGameplayScreen()
    {
        SceneManager.LoadScene(GAMEPLAY_BUTTON_SCREEN_NAME);
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}