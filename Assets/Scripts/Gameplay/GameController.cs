using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerCon playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

   public GameState state;

    public static GameController Instance{get; private set;}    
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerController.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerController));
            }
        };

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

    void StartBattle()
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
    //Start Trainer Battle
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


    void EndBattle(bool won)
    {
        if(trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
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
            // Pause movement when entering cutscene
            playerController.PauseMovement();
        }
        else
        {
            // Resume movement when leaving cutscene
            playerController.ResumeMovement();
        }
    }

}
