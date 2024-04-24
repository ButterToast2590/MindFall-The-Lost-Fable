


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownsprites;
    [SerializeField] List<Sprite> walkUpsprites;
    [SerializeField] List<Sprite> walkRightsprites;
    [SerializeField] List<Sprite> walkLeftsprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;


    // parameters
    public float MoveX {  get; set; }
    public float MoveY {  get; set; }
    public bool IsMoving {  get; set; }

    //States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator currentAnim;

    // Reference
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownsprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpsprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightsprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftsprites, spriteRenderer);
        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;
        if (MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1)
        {
            currentAnim = walkLeftAnim;
        }
        else if (MoveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if (MoveY == -1)
        {
            currentAnim = walkDownAnim;
        }

        if (currentAnim != prevAnim)
        {
            currentAnim.Start();
        }

        if (IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        // Reset both MoveX and MoveY to zero before setting the new direction
        MoveX = 0;
        MoveY = 0;

        // Set the corresponding direction property based on the enum value
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
    }


    public FacingDirection DefaultDirection { get => defaultDirection;}
}

public enum FacingDirection {Up, Down, Left, Right , None};


