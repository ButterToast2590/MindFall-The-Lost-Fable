using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public Character Character { get; private set; }

    private void Update()
    {
        if (Character == null)
        {
            Transform current = transform;
            while (current != null)
            {
                Debug.Log("Checking GameObject: " + current.gameObject.name);
                Character = current.GetComponent<Character>();
                if (Character != null)
                {
                    Debug.Log("Character component found on GameObject: " + current.gameObject.name);
                    break;
                }
                current = current.parent;
            }

            if (Character == null)
            {
                Debug.LogError("Character component not found in parent hierarchy.");
            }
        }
    }

    public void OnPlayerTriggered(PlayerCon player)
    {
            GameController.Instance.OnEnterTrainersView(GetComponentInParent<TrainerController>());
    }
}
