using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Import the SceneManager class

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(dialogueSequence());
        }

        private IEnumerator dialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);
                yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);
            }
            // Load the main game scene after the dialogue sequence is finished
            LoadMainScene();
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("SampleScene"); // Replace "MainGameSceneName" with the name of your main game scene
        }

        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
