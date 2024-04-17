using System;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed;
    public event Action OnEncountered;
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Update()
    {
        HandleUpdate();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            Vector2 input = Vector2.zero;
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input * moveSpeed, CheckForEncounters));
            }
            else
            {
                float moveX = character.Animator.MoveX;
                float moveY = character.Animator.MoveY;
                Vector3 targetPos = transform.position + new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;
                if (character.IsWalkable(targetPos))
                {
                    transform.Translate(new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 15)
            {
                character.Animator.IsMoving = false;
                OnEncountered?.Invoke();
            }
        }
    }

    // Methods for button input handling
    public void RightBtnDown()
    {
        character.Animator.MoveX = 1;
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = true;
    }

    public void RightBtnUp()
    {
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = false;
        CheckForEncounters();
    }

    public void LeftBtnDown()
    {
        character.Animator.MoveX = -1;
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = true;
    }

    public void LeftBtnUp()
    {
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = false;
        CheckForEncounters();
    }

    public void UpBtnDown()
    {
        character.Animator.MoveY = 1;
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = true;
    }

    public void UpBtnUp()
    {
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = false;
        CheckForEncounters();
    }

    public void DownBtnDown()
    {
        character.Animator.MoveY = -1;
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = true;
    }

    public void DownBtnUp()
    {
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = false;
        CheckForEncounters();
    }

    // Method for interacting with objects
    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public void OnInteract()
    {
        Interact();
    }
}
