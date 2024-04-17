using System;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed;

    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;
    public bool Moving;
    private Animator animator;
    public LayerMask Obstacles;
    public LayerMask interactableLayer; // Changed to public
    public LayerMask grassLayer;

    public event Action OnEncountered;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Left)
        {
            animator.SetFloat("moveX", -1f);
            animator.SetFloat("moveY", 0f);
            moveX = -1f;
        }
        if (Right)
        {
            animator.SetFloat("moveX", 1f);
            animator.SetFloat("moveY", 0f);
            moveX = 1f;
        }

        if (Up)
        {
            animator.SetFloat("moveY", 1f);
            animator.SetFloat("moveX", 0f);
            moveY = 1f;
        }
        if (Down)
        {
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveY", -1f);
            moveY = -1f;
        }

        // Move the player
        Vector3 targetPos = transform.position + new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;
        if (IsWalkable(targetPos))
        {
            transform.Translate(new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime);
        }
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 15)
            {
                animator.SetBool("Moving", false);
                OnEncountered();
            }
        }
    }

    //boolean
    public void RightBtnDown()
    {
        Right = true;
        animator.SetBool("Moving", true);
    }

    public void RightBtnUp()
    {
        Right = false;

        if (!Left && !Up && !Down)
        {
            animator.SetFloat("moveX", 1); // No horizontal movement, set to idle
        }
        else if (!Left)
        {
            animator.SetFloat("moveY", -1); // Switch to idle left animation if not moving left
        }
        animator.SetBool("Moving", false);
        CheckForEncounters();
    }

    public void LeftBtnDown()
    {
        Left = true;
        animator.SetBool("Moving", true);
    }

    public void LeftBtnUp()
    {
        Left = false;

        if (!Right && !Up && !Down)
        {
            animator.SetFloat("moveX", -1); // No horizontal movement, set to idle facing left
            animator.SetFloat("moveY", 0);
        }
        else if (!Right)
        {
            animator.SetFloat("moveY", 1); // Switch to idle right animation if not moving right
        }
        animator.SetBool("Moving", false);
        CheckForEncounters();
    }

    public void UpBtnDown()
    {
        Up = true;
        animator.SetBool("Moving", true);
    }

    public void UpBtnUp()
    {
        Up = false;

        if (!Right && !Left && !Down)
        {
            animator.SetFloat("moveY", 1);
            animator.SetFloat("moveX", 0);// No vertical movement, set to idle facing up
        }
        else if (!Down)
        {
            animator.SetFloat("moveX", -1); // Switch to idle facing up animation if not moving down
        }
        animator.SetBool("Moving", false);
        CheckForEncounters();
    }

    public void DownBtnDown()
    {
        Down = true;
        animator.SetBool("Moving", true);
    }

    public void DownBtnUp()
    {
        Down = false;

        if (!Right && !Left && !Up)
        {
            animator.SetFloat("moveY", -1); // No vertical movement, set to idle facing down
            animator.SetFloat("moveX", 0);
        }
        else if (!Up)
        {
            animator.SetFloat("moveX", 1); // Switch to idle facing down animation if not moving up
        }
        animator.SetBool("Moving", false);
        CheckForEncounters();
    }
    //boolean end here

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, Obstacles | interactableLayer) != null) // Changed to use the bitwise OR operator
        {
            return false;
        }

        return true;
    }

    void interact()
    {
        var animator = GetComponent<Animator>(); // Get the Animator component

        if (animator != null) // Check if Animator component exists
        {
            var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            var interactPos = transform.position + facingDir;

            var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
            if (collider != null)
            {
                collider.GetComponent<Interactable>()?.Interact();
            }
        }
        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
    }

    public void OnInteract()
    {
        interact();
    }
}
