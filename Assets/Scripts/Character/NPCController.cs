using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [SerializeField] Sprite npcImage; // Serialized field for the NPC's image
    [SerializeField] string npcName; // Serialized field for the NPC's name

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;

    // Properties to access NPC's name and image
    public Sprite NPCImage { get { return npcImage; } }
    public string NPCName { get { return npcName; } }

    public void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, npcName, npcImage, () => {
                    idleTimer = 0f;
                    state = NPCState.Idle;
            }));
        }
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;    

        yield return character.Move(movementPattern[currentPattern]);
        if (transform.position != oldPos)
        {
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        }

        state = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog}










