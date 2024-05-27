using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private CharacterAnimator animator;
    public float OffsetY { get; private set; } = 0.3f;
    public bool IsMoving { get; private set; }

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y + OffsetY) + 0.5f;

        transform.position = pos;
    }

    public IEnumerator Move(Vector2 moveVec, System.Action OnMoveOver = null)
    {
        IsMoving = true;

        Vector3 targetPos = transform.position + new Vector3(moveVec.x, moveVec.y, 0f);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        IsMoving = false; 
        animator.IsMoving = false;

        OnMoveOver?.Invoke();
    }


    public void HandleUpdate() // Moved this method inside the class
    {
        animator.IsMoving = isMoving;
    }

    private bool IsPathClear(Vector3 targetPos) // Moved this method inside the class
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.ObstaclesLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer))
        {
            return false;
        }
        return true;
    }

    public bool IsWalkable(Vector2 targetPos) // Moved this method inside the class
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.ObstaclesLayer | GameLayers.i.InteractableLayer))
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos) // Moved this method inside the class
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xdiff == 0 || ydiff == 0)
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);
        }
        else
        {
            Debug.LogError("Error in Look Towards: You can't ask the character to look diagonally");
        }
    }

    public CharacterAnimator Animator { get => animator; } // Moved this property inside the class
}
