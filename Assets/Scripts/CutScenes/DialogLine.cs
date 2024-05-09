using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogLine : DialogBaseClass
    {
        private Text textHolder;
        [SerializeField] private string input;

        private void Start()
        {
            textHolder = GetComponent<Text>();

            if (textHolder != null)
            {
                StartCoroutine(WriterText(input, textHolder));
            }
            else
            {
                Debug.LogError("Text component not found on the GameObject: " + gameObject.name);
            }
        }

    }
}
