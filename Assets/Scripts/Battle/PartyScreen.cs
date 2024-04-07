using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button backButton;
    [SerializeField] GameObject partyButtonContainer;
    [SerializeField] PartyMemberUI[] memberSlots;

    List<Fables> fables;

    void Start()
    {
        Init(); // Initialize member slots
        HideBackButton(); // Hide back button initially
    }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Fables> fables)
    {
        this.fables = fables;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < fables.Count)
                memberSlots[i].SetData(fables[i]);
            else
                memberSlots[i].gameObject.SetActive(false); // Deactivate excess slots
        }

        messageText.text = "Choose a Fable";
    }

    public void ShowPartyScreen()
    {
        gameObject.SetActive(true);
        partyButtonContainer.SetActive(true);
        ShowBackButton();
        HideBackButtonInBattle();
    }

    public void HidePartyScreen()
    {
        gameObject.SetActive(false);
        partyButtonContainer.SetActive(false);
        HideBackButton();
        ShowBackButtonInBattle();
    }

    void ShowBackButton()
    {
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }
    }

    void HideBackButton()
    {
        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
        }
    }

    void HideBackButtonInBattle()
    {
        if (backButton != null)
        {
            backButton.gameObject.SetActive(false);
        }
    }

    void ShowBackButtonInBattle()
    {
        if (backButton != null)
        {
            backButton.gameObject.SetActive(true);
        }
    }

    public void OnBackButtonClick()
    {
        HidePartyScreen();
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < fables.Count; i++)
        {
            if (i == selectedMember)
                memberSlots[i].SetSelected(true);
            else
                memberSlots[i].SetSelected(false);
        }
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }

    public void SelectPartyMember(int selectedIndex)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            memberSlots[i].SetSelected(i == selectedIndex);
        }
    }

    public PartyMemberUI[] GetMemberSlots()
    {
        return memberSlots;
    }
}
