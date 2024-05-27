using System;
using UnityEngine;

public class PlayerCon : MonoBehaviour, IDataPersistence
{

    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    public float moveSpeed;
    public event Action<Collider2D> OnEnterTrainersView;

    private Character character;
    bool isInDialog = false;
    bool isMovementPaused = false;
    bool isInTrainerViewCollider = false;


    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
    private const float offsetY = 0.0f;

    public Character Character
    {
        get
        {
            if (character == null)
            {
                character = GetComponent<Character>();
            }
            return character;
        }
    }

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
        if (!character.IsMoving && !isMovementPaused) 
        {
            Vector2 input = Vector2.zero;
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            CheckIfInTrainersView();
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input * moveSpeed, OnMoveOver));
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
    public void LoadData(GameData data)
    {
        this.transform.position = data.playerPosition;
    }

    public void SaveData(ref GameData data)
    {
        data.playerPosition = this.transform.position;
    }

    private void CheckIfInTrainersView()
    {
        if (!isInDialog && !isMovementPaused)
        {
            var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
            if (collider != null)
            {
                // Check if the player has just entered the collider
                if (!isInTrainerViewCollider)
                {
                    character.Animator.IsMoving = false;
                    OnEnterTrainersView?.Invoke(collider);
                    // Pause movement when entering the trainer's view collider
                    PauseMovement();
                }
                isInTrainerViewCollider = true;
            }
            else
            {
                // Resume movement when leaving the trainer's view collider
                if (isInTrainerViewCollider)
                {
                    ResumeMovement();
                }
                isInTrainerViewCollider = false;
            }
        }
    }



    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.TriggerLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                Debug.Log("Trigger detected: " + collider.name); // Add this line for debugging
                break;
            }
        }
    }



    public void PauseMovement()
    {
        isMovementPaused = true;
    }

    public void ResumeMovement()
    {
        isMovementPaused = false;
    }

    // Methods for button input handling
    public void RightBtnDown()
    {
        character.Animator.MoveX = 1;
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = true;
        OnMoveOver();
    }

    public void RightBtnUp()
    {
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = false;
    }

    public void LeftBtnDown()
    {
        character.Animator.MoveX = -1;
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = true;
        OnMoveOver();
    }

    public void LeftBtnUp()
    {
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = false;
    }

    public void UpBtnDown()
    {
        character.Animator.MoveY = 1;
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = true;
        OnMoveOver();
    }

    public void UpBtnUp()
    {
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = false;
    }

    public void DownBtnDown()
    {
        character.Animator.MoveY = -1;
        character.Animator.MoveX = 0;
        character.Animator.IsMoving = true;
        OnMoveOver();
    }

    public void DownBtnUp()
    {
        character.Animator.MoveY = 0;
        character.Animator.IsMoving = false;
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