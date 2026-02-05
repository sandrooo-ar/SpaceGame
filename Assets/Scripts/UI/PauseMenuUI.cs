using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    public GameObject _canvasHolder;
    
    private PlayerInput playerInput;

    private bool _isPaused = false;

    private void Awake()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        // Subscribe to input action
        playerInput.actions["Pause"].performed += OnPausePressed;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (playerInput != null)
            playerInput.actions["Pause"].performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        // Toggle pause state
        if (_isPaused)
            ResumeGame();
        else
            ShowMenu();
    }

    public void GoToMainMenu()
    {
        // Player death functionality
        if (GameObject.FindGameObjectWithTag("LevelManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("LevelManager"));
        }

        if (GameObject.FindGameObjectWithTag("CoinManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("CoinManager"));
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }

        if (GameObject.FindGameObjectWithTag("UpgradeSystem") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("UpgradeSystem"));
        }

        Time.timeScale = 1f;
        GameManager.Instance.LoadLevel("MainMenu");
    }

    public void RestartLevel()
    {

        // Player death functionality
        if (GameObject.FindGameObjectWithTag("LevelManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("LevelManager"));
        }

        if (GameObject.FindGameObjectWithTag("CoinManager") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("CoinManager"));
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Player"));
        }

        if (GameObject.FindGameObjectWithTag("UpgradeSystem") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("UpgradeSystem"));
        }

        Time.timeScale = 1f;
        GameManager.Instance.LoadLevel("Grassland");
    }

    public void ResumeGame()
    {
        HideMenu();
    }

    public void ShowMenu()
    {
        Time.timeScale = 0f;
        _isPaused = true;

        _canvasHolder.transform.localScale = Vector3.zero;
        _canvasHolder.transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // ignores timescale
    }

    public void HideMenu()
    {
        _canvasHolder.transform.DOScale(Vector3.zero, 0.2f)
            .SetEase(Ease.InBack)
            .SetUpdate(true); // ignores timescale

        Time.timeScale = 1f;
        _isPaused = false;
    }
}
