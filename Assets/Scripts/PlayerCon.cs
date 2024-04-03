using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour
{
    public float moveSpeed;

    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;
    private Animator animator;
    public LayerMask Obstacles;
    public LayerMask grassLayer;



    public event Action OnEncountered;

    // Start is called before the first frame update
    void Start()
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
            moveX = -1f;
        }
        else if (Right)
        {
            moveX = 1f;
        }

        if (Up)
        {
            moveY = 1f;
        }
        else if (Down)
        {
            moveY = -1f;
        }

        // Set the animation parameters
        animator.SetFloat("moveX", moveX);
        animator.SetFloat("moveY", moveY);
        animator.SetBool("Moving", moveX != 0 || moveY != 0); // Check if moving

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
   
        public void RightBtnDown()
        {
            Right = true;
        }
        public void RightBtnUp()
        {
            Right = false;
            if (!Left && !Up && !Down)
            {
                animator.SetFloat("moveX", 0); // No horizontal movement, set to idle
            }
        CheckForEncounters();
        }

        public void LeftBtnDown()
        {
            Left = true;
        }
        public void LeftBtnUp()
        {
            Left = false;
            if (!Right && !Up && !Down)
            {
                animator.SetFloat("moveX", 0); // No horizontal movement, set to idle
            }
        CheckForEncounters();
        }

        public void UpBtnDown()
        {
            Up = true;
        }
        public void UpBtnUp()
        {
            Up = false;
            if (!Right && !Left && !Down)
            {
                animator.SetFloat("moveY", -1); // No vertical movement, set to idle facing down
            }
        CheckForEncounters();
        }

        public void DownBtnDown()
        {
            Down = true;
        }
        public void DownBtnUp()
        {
            Down = false;
            if (!Right && !Left && !Up)
            {
                animator.SetFloat("moveY", -1); // No vertical movement, set to idle facing down
            }
        CheckForEncounters();
        }


    private bool IsWalkable(Vector3 targetPos)
    {
        // Cast a ray from the current position to the target position
        RaycastHit2D hit = Physics2D.Linecast(transform.position, targetPos, Obstacles);

        // If the ray hits an obstacle, return false
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }
}
