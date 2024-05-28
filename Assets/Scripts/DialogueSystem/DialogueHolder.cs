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

        private Scene currentScene;

        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene();
            StartCoroutine(DialogueSequence());
        }

        private IEnumerator DialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                transform.GetChild(i).gameObject.SetActive(true);
                backgroundImageHolder.sprite = backgroundImages[i];
                yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);
            }
            if (currentScene.name == "Route1")
            {
                LoadEndScene();
            }
            else if (currentScene.name == "EndScene")
            {
                LoadTitleScene();
            }
            else
            {
                LoadMainScene();
            }
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("HomeTown");
        }

        private void LoadEndScene()
        {
            SceneManager.LoadScene("EndScene");
        }
        private void LoadTitleScene()
        {
            SceneManager.LoadScene("TitleScreen");
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
