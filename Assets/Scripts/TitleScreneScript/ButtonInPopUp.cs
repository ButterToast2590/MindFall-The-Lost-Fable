using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonInPopUp : MonoBehaviour
{
    public GameObject PlayGamePanel;

    public void ContinueGame()
    {
        // Handle continue action
        SceneManager.LoadScene("HomeTown"); // Example scene name for continuing game
    }

    public void NewGame()
    {
        // Handle new game action
        SceneManager.LoadScene("IntroScene"); // Example scene name for new game
    }

    public void ClosePopup()
    {
        // Close the popup panel
        PlayGamePanel.SetActive(false);
    }
}
