using UnityEngine;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject skillSelectionPanel;

    public void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);
        skillSelectionPanel.SetActive(false);
    }

    public void OpenSkillSelection()
    {
        mainMenuPanel.SetActive(false);
        skillSelectionPanel.SetActive(true);
    }

    public void StartGame()
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

        GameManager.Instance.LoadLevel("Grassland");
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }

}
