using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [Header("Background Images")]
        [SerializeField] private Sprite[] backgroundImages;
        [SerializeField] private Image backgroundImageHolder;

        private void Awake()
        {
            StartCoroutine(DialogueSequence());
        }

        private IEnumerator DialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);
                backgroundImageHolder.sprite = backgroundImages[i]; // Change background image
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
