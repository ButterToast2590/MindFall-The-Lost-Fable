using System.Collections;
using TMPro;
using UnityEngine;
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    Dialog dialog;
    int currentLine = 0;
    bool isTyping;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        if (isTyping)
        {
            // If currently typing a line, check for user input to skip the animation
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Stop the typing coroutine and display the entire line immediately
                StopCoroutine("TypeDialog");
                isTyping = false;
                dialogText.text = dialog.Lines[currentLine];
            }
        }
        else
        {
            // If not typing, proceed to the next line of dialog
            if (currentLine < dialog.Lines.Count)
            {
                dialogText.text = ""; // Clear the dialog text
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                currentLine++;
            }
            else
            {
                // If there are no more lines, close the dialog
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
