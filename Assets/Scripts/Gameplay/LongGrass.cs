using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerCon player)
    {
        Debug.Log("Enter Long Grass");
        if (UnityEngine.Random.Range(1, 101) <= 15)
        {
            Debug.Log("LongGrass: Starting a battle."); // Add this debug log
            GameController.Instance.StartBattle();
        }
    }
}
