using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ImportantNPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog initialDialog;
    [SerializeField] Dialog afterDialog;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    [SerializeField] Sprite npcImage; 
    [SerializeField] string npcName; 
    [SerializeField] string sceneToLoad;

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;
    Character character;
    bool hasInteractedBefore = false;

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

            Dialog dialogToShow = hasInteractedBefore ? afterDialog : initialDialog;
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogToShow, npcName, npcImage, () => {
                idleTimer = 0f;
                state = NPCState.Idle;

                if (!hasInteractedBefore)
                {
                    hasInteractedBefore = true;

                    if (!string.IsNullOrEmpty(sceneToLoad))
                    {
                        SceneManager.LoadScene(sceneToLoad);
                    }
                }
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

public enum StoryNPCState { Idle, Walking, Dialog }
