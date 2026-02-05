using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public bool gamePaused = false;

    public string currentLevelName = "Level1";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadLevel(string levelName)
    {
        DOTween.KillAll();
        SceneManager.LoadScene(levelName);
        currentLevelName = levelName;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        // Kill all active tweens before unloading the scene
        DOTween.KillAll();
        SceneManager.LoadScene("MainMenu");
        currentLevelName = "MainMenu";
    }

    public void RestartLevel(string levelName)
    {
        // Kill all active tweens before unloading the scene
        DOTween.KillAll();
        SceneManager.LoadScene(levelName);
        currentLevelName = levelName;
    }

}
