using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonsGame : MonoBehaviour
{
    public GameObject PlayGamePanel;

    public void PlayGame()
    {
        // Show the popup panel
        PlayGamePanel.SetActive(true);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
