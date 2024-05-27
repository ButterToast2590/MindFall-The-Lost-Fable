using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerCon playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    public GameState state;

    GameState stateBeforePause;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        ConditionsDB.Init();
    }

    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
            {
                state = GameState.FreeRoam;
            }
        };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;
        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<FableParty>();
        var wildFable = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildFables();

        var wildFableCopy = new Fables(wildFable.Base, wildFable.Level);

        battleSystem.StartBattle(playerParty, wildFableCopy);
    }

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;
        var playerParty = playerController.GetComponent<FableParty>();
        var trainerParty = trainer.GetComponent<FableParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    public void OnEnterTrainersView(TrainerController trainer)
    {
        Debug.Log("OnEnterTrainersView called");
        if (state != GameState.Cutscene)
        {
            state = GameState.Cutscene;
            StartCoroutine(trainer.TriggerTrainerBattle(playerController));
        }
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        battleSystem.ResetBattleState();

        if (won && trainer != null)
        {
            trainer.BattleLost();
        }
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Cutscene)
        {
            StartTrainerBattle(trainer);
        }
    }

    public void StartCutsceneState()
    {
        state = GameState.Cutscene;
    }

    public void StartFreeRoamState()
    {
        state = GameState.FreeRoam;
    }
}
