using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;
    Character character;
    [SerializeField] GameController gameController;
    [SerializeField] TrainerController trainerController;


    private void Awake()
    {
        character = GetComponent<Character>();
    }
    private void Start()
    {
        SetGameController(gameController);
        SetFovRotation(character.Animator.DefaultDirection);
    }
 
    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }


    public IEnumerator TriggerTrainerBattle(PlayerCon player)
    {
        // Pause player movement
        player.PauseMovement();

        // Set the game state to Cutscene if GameController reference is not null
        if (gameController != null)
        {
            gameController.state = GameState.Cutscene;
        }
        else
        {
            Debug.LogError("GameController reference is null in TrainerController.");
        }

        // Show the Exclamation and disable it after 0.5 sec
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // This makes the trainer move from its position to the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        // Wait for the trainer's animation
        yield return character.Move(moveVec);

        // Show dialog before battle
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, null, null, () =>
        {
            Debug.Log("hoyy bakla");
            // Resume player movement after the dialog
            player.ResumeMovement();
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

}
