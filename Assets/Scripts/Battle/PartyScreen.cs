using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] Button backButton;
    [SerializeField] GameObject partyButtonContainer;
    PartyMemberUI[] memberSlots;

    List<Fables> fables;
    int selectedMemberIndex = -1;

    void Start()
    {
        Init();
        HideBackButton();
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
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Fable";
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
    public void SelectPartyMember(int selectedIndex)
    {
        selectedMemberIndex = selectedIndex; 
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
