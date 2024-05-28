using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public enum DestinationIdentifier { A, B, C, D, E }
public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationIdentifier;
    [SerializeField] Transform spawnPoint;

    PlayerCon player;
    public Character Character { get; private set; }

    private void Awake()
    {
        // Check if the Character component is present
        Character = GetComponent<Character>();
        if (Character == null)
        {
            Debug.LogError("Character component not found on the portal object.");
        }
    }


    public void OnPlayerTriggered(PlayerCon player)
    {
        if (player == null)
        {
            Debug.LogError("PlayerCon reference is null.");
            return;
        }

        if (player.Character == null)
        {
            Debug.LogError("Character component not found on the player object.");
            return;
        }
        this.player = player;
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().FirstOrDefault(x => x != this && x.destinationIdentifier == this.destinationIdentifier);
        if (destPortal != null)
        {
            player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        }
        else
        {
            Debug.LogError("No destination portal found for identifier: " + destinationIdentifier);
        }

        if (GameController.Instance.state == GameState.Paused)
        {
            GameController.Instance.PauseGame(false);
        }
        else
        {
            Debug.LogWarning("Game state was not properly restored after scene transition.");
        }
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}