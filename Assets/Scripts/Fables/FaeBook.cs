using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaeBook : MonoBehaviour
{
    public GameObject[] entries;
    public GameObject[] playerTips; 
    public GameObject[] menuItems; 

    public void EnableEntry(int id)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            entries[i].SetActive(false);
        }
        entries[id].SetActive(true);
    }

    public void EnablePlayerTip(int id) 
    {
        for (int i = 0; i < playerTips.Length; i++)
        {
            playerTips[i].SetActive(false);
        }
        playerTips[id].SetActive(true);
    }

    public void EnableMenuItem(int id) 
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetActive(false);
        }
        menuItems[id].SetActive(true);
    }
}
