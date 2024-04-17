using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 
using System;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Button continueButton; 

    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    Dialog dialog;
    Action onDialogFinished;
    int currentLine = 0;
    bool isTyping;

    public bool IsShowing { get; private set; }


    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        currentLine = 0;
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public void HandleUpdate()
    {
        bool showContinueButton = !isTyping && currentLine < dialog.Lines.Count;
        continueButton.gameObject.SetActive(showContinueButton);
    }

    public void ContinueButtonPressed()
    {
        if (!isTyping && currentLine < dialog.Lines.Count - 1)
        {
            // Go to the next line
            currentLine++;
            dialogText.text = "";
            StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        }
        else if (!isTyping && currentLine == dialog.Lines.Count - 1)
        {
            CloseDialog();
        }
    }

    private void CloseDialog()
    {
        currentLine = 0;
        IsShowing = false;
        dialogBox.SetActive(false);
        onDialogFinished?.Invoke(); 
        OnCloseDialog?.Invoke();
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
