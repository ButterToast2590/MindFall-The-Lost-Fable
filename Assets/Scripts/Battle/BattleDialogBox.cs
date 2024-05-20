using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Button> actionButtons;
    [SerializeField] List<Button> moveButtons;

    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI typeText;
    [SerializeField] TextMeshProUGUI yesText;
    [SerializeField] TextMeshProUGUI noText;

    private Move currentMove;
    public Text MoveDetailsText;

    private void Awake()
    {
        moveSelector.SetActive(false);
        moveDetails.SetActive(false); // Hide move details initially
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableMoveDetails(bool enabled)
    {
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionButtons.Count; ++i)
        {
            Color color = (i == selectedAction) ? highlightedColor : Color.white;
            actionButtons[i].image.color = color;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < actionButtons.Count; ++i)
        {
            Color color = (i == selectedMove) ? highlightedColor : Color.white;
            actionButtons[i].image.color = color;
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveButtons.Count; ++i)
        {
            if (i < moves.Count)
            {
                TextMeshProUGUI buttonText = moveButtons[i].GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonText != null)
                {
                    buttonText.text = moves[i].Base.Name;

                    int moveIndex = i;
                    moveButtons[i].onClick.RemoveAllListeners();
                    moveButtons[i].onClick.AddListener(() => {
                        UpdateMoveDetails(moves[moveIndex]);
                    });

                    // Add Tooltip component and set the message
                    Tooltip tooltip = moveButtons[i].gameObject.GetComponent<Tooltip>() ?? moveButtons[i].gameObject.AddComponent<Tooltip>();
                    string tooltipMessage = $"Move: {moves[i].Base.Name}\nPP: {moves[i].PP}/{moves[i].Base.PP}\nType: {moves[i].Base.Type}";
                    tooltip.UpdateMessage(tooltipMessage);

                    // Add EventTrigger to handle long press for tooltip
                    EventTrigger trigger = moveButtons[i].gameObject.GetComponent<EventTrigger>() ?? moveButtons[i].gameObject.AddComponent<EventTrigger>();

                    EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                    pointerDownEntry.callback.AddListener((data) => { tooltip.OnPointerDown((PointerEventData)data); });
                    trigger.triggers.Add(pointerDownEntry);

                    EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                    pointerUpEntry.callback.AddListener((data) => { tooltip.OnPointerUp((PointerEventData)data); });
                    trigger.triggers.Add(pointerUpEntry);
                }
            }
            else
            {
                TextMeshProUGUI buttonText = moveButtons[i].GetComponentInChildren<TextMeshProUGUI>(true);
                if (buttonText != null)
                {
                    buttonText.text = "-";
                }
            }
        }
    }

    public int CalculateSelectedMoveIndex(Vector2 inputPosition)
    {
        int moveIndex = -1;

        // Determine the selected move button based on input position
        for (int i = 0; i < moveButtons.Count; ++i)
        {
            RectTransform buttonRect = moveButtons[i].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(buttonRect, inputPosition))
            {
                moveIndex = i;
                break;
            }
        }

        return moveIndex;
    }

    private void OnMoveButtonClick(Move move)
    {
        UpdateMoveDetails(move);
    }

    public void UpdateMoveDetails(Move move)
    {
        if (move != null)
        {
            ppText.text = $"PP: {move.PP}/{move.Base.PP}";
            typeText.text = $"Type: {move.Base.Type.ToString()}";
            moveDetails.SetActive(true);

            if (move.PP == 0)
            {
                ppText.color = Color.red;
            }
            else
            {
                ppText.color = Color.black;
            }
        }
    }
}
