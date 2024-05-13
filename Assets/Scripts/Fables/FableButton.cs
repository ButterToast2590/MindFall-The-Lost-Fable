using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FableButton : MonoBehaviour
{
    public int fableID; // Assign this in the Unity Editor
    public FaeBook faeBook; // Reference to the FaeBook script

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        faeBook.EnableEntry(fableID); // Enable entry for the clicked fable
    }
}