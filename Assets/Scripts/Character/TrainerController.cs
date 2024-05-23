using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    Character character;
    [SerializeField] GameController gameController;

    // State
    bool battleLost = false;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetGameController(gameController);
        SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        character.HandleUpdate();
    }

    public void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);
        if (!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
            {
                GameController.Instance.StartTrainerBattle(this);
                initiator.GetComponent<PlayerCon>().ResumeMovement();
            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle, name, sprite));
        }
    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
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

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }

    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    public IEnumerator TriggerTrainerBattle(PlayerCon player)
    {
        player.PauseMovement();

        if (gameController != null)
        {
            gameController.state = GameState.Cutscene;
        }
        else
        {
            Debug.LogError("GameController reference is null in TrainerController.");
        }

        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Calculate the direction from the trainer to the player
        var diff = player.transform.position - transform.position;

        // Move the trainer towards the player until one tile away
        while (diff.magnitude > 1.0f)
        {
            var moveDir = diff.normalized;

            yield return character.Move(moveDir);
            diff = player.transform.position - transform.position;
        }

        // Show dialog before battle
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
        {
            GameController.Instance.StartTrainerBattle(this);
            player.ResumeMovement();
        }));
    }
}
