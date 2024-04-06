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

    PartyMemberUI[] memberSlots;

    void Start()
    {
        HideBackButton(); // Initially hide the back button
    }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Fables> fables)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < fables.Count)
                memberSlots[i].SetData(fables[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
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
}
