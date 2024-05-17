using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Make sure to include this namespace

public class SceneLoaderController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;
    [SerializeField] string sceneToLoad; // Add a variable to specify the scene to load
    Character character;
    [SerializeField] GameController gameController;

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
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
        {
            LoadScene();
            // Resume player movement after the dialog
            initiator.GetComponent<PlayerCon>().ResumeMovement();
        }));
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void SetGameController(GameController controller)
    {
        gameController = controller;
    }

    public IEnumerator TriggerSceneLoad(PlayerCon player)
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
            Debug.LogError("GameController reference is null in SceneLoaderController.");
        }

        // Show the Exclamation and disable it after 0.5 sec
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Calculate the direction from the NPC to the player
        var diff = player.transform.position - transform.position;

        // Move the NPC towards the player
        while (diff.magnitude > 1.0f) // Adjust this threshold as needed
        {
            // Calculate the movement direction
            var moveDir = diff.normalized;

            // Move the NPC one step towards the player
            yield return character.Move(moveDir);

            // Recalculate the direction to the player
            diff = player.transform.position - transform.position;
        }

        // Show dialog before loading the scene
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, () =>
        {
            LoadScene();
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

    public string Name { get => name; }
    public Sprite Sprite { get => sprite; }
}
