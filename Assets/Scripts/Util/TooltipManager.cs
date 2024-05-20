using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _Instance;

    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI MoveDetails;

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Instance = this;
        }
    }

    void Start()
    {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetAndShowToolTip(string message)
    {
        gameObject.SetActive(true);
        textComponent.text = message;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

    public void UpdateToolTip(string message)
    {
        if (gameObject.activeSelf)
        {
            textComponent.text = message;
        }
    }
}
