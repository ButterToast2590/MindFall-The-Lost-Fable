using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    private bool triggered = false;

    public void OnPlayerTriggered(PlayerCon player)
    {
        if (!triggered)
        {
            Debug.Log("Player entered trainer's FOV");
            triggered = true;
            GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
        }
    }
}
