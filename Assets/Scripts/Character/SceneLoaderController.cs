using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Dialog dialog;
    [SerializeField] string sceneToLoad; // Variable to specify the scene to load
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
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, LoadScene));
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
        if (gameController != null)
        {
            gameController.state = GameState.Cutscene;
        }
        else
        {
            Debug.LogError("GameController reference is null in SceneLoaderController.");
        }
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        var diff = player.transform.position - transform.position;
        while (diff.magnitude > 1.0f) 
        {
            var moveDir = diff.normalized;
            yield return character.Move(moveDir);
            diff = player.transform.position - transform.position;
        }

        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, name, sprite, LoadScene));
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
