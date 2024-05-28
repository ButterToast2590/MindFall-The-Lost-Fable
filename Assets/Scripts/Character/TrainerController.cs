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
        if (!battleLost) // Check if the trainer has not been defeated
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
            {
                if (!battleLost) // Check again before starting the battle
                {
                    GameController.Instance.StartTrainerBattle(this);
                }
                else
                {
                    StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle, name, sprite));
                }
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

    public IEnumerator TriggerTrainerBattle(PlayerCon player, float minDistance = 1.5f)
    {
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Calculate the direction from the trainer to the player
        var diff = player.transform.position - transform.position;

        // Move the trainer towards the player while maintaining the minimum distance
        while (diff.magnitude > minDistance)
        {
            var moveDir = diff.normalized;
            Debug.Log("Moving towards player: " + moveDir);

            yield return StartCoroutine(character.Move(moveDir, () => Debug.Log("Movement step completed")));

            // Recalculate the direction and distance after movement
            diff = player.transform.position - transform.position;
            Debug.Log("Updated difference: " + diff);
        }

        // After reaching the minimum distance, start the dialog
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
        {
            if (GameController.Instance != null)
            {
                GameController.Instance.StartTrainerBattle(this);
            }
            else
            {
                Debug.LogError("GameController reference is null, unable to start battle.");
            }
        }));
    }



}
