using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string message;
    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;
    [SerializeField] private float requiredHoldTime = 1f;

    private void Update()
    {
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime)
            {
                TooltipManager._Instance.SetAndShowToolTip(message);
                isPointerDown = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        pointerDownTimer = 0f;
        TooltipManager._Instance.HideToolTip();
    }

    public void UpdateMessage(string newMessage)
    {
        message = newMessage;
    }
}
