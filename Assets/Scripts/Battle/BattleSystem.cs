using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int selectedMoveIndex; // Variable to store the selected move index
    bool moveSelected = false;


    FableParty playerParty;
    Fables wildFables;


    public void StartBattle(FableParty playerParty, Fables wildFables)
    {
        this.playerParty = playerParty;
        this.wildFables = wildFables;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyFable());
        enemyUnit.Setup(wildFables);
        playerHUD.SetData(playerUnit.fables);
        enemyHUD.SetData(enemyUnit.fables);

        dialogBox.SetMoveNames(playerUnit.fables.Moves);

        yield return dialogBox.TypeDialog($"A Wild {enemyUnit.fables.Base.FableName} appeared.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        moveSelected = false; 
        StartCoroutine(HandlePlayerAction());
    }
    void DisableMoveDetails()
    {
        dialogBox.EnableMoveDetails(false);
        dialogBox.EnableActionSelector(false); 
    }

    IEnumerator HandlePlayerAction()
    {
        yield return StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.fables.Moves[selectedMoveIndex];
        move.PP--;

        yield return dialogBox.TypeDialog($"{playerUnit.fables.Base.FableName} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();
        var damageDetails = enemyUnit.fables.TakeDamage(move, playerUnit.fables);
        yield return enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.fables.Base.FableName} Fainted");
            enemyUnit.PlayFaintAnimation();


            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
        dialogBox.EnableMoveDetails(false);
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.fables.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.fables.Base.FableName} used {move.Base.Name}");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();
        var damageDetails = playerUnit.fables.TakeDamage(move, enemyUnit.fables);
        yield return playerHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.fables.Base.FableName} Fainted");
            playerUnit.PlayFaintAnimation();


            yield return new WaitForSeconds(2f);

            var nextFable = playerParty.GetHealthyFable();
            if (nextFable != null)
            {
                playerUnit.Setup(nextFable);
                playerHUD.SetData(nextFable);

                dialogBox.SetMoveNames(nextFable.Moves);

                yield return dialogBox.TypeDialog($"I choose you Go {nextFable.Base.FableName}!");

                PlayerAction();
            }
            else
            {
                OnBattleOver(false);
            }

        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!!");
            
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!!");
    }



    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandlePlayerAction();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleMoveSelection()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Vector2 inputPosition = (Input.touchCount > 0) ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            selectedMoveIndex = dialogBox.CalculateSelectedMoveIndex(inputPosition); // Update the selected move index

            if (!moveSelected)
            {
                dialogBox.UpdateMoveSelection(selectedMoveIndex, playerUnit.fables.Moves[selectedMoveIndex]);
                dialogBox.EnableMoveDetails(true); // Enable move details here
                moveSelected = true;
            }
            else
            {
                ConfirmMoveSelection(selectedMoveIndex);
                // Disable move details here instead of in ConfirmMoveSelection
                DisableMoveDetails();
            }
        }
    }


    void ConfirmMoveSelection(int selectedMoveIndex)
    {
        moveSelected = false;
        dialogBox.UpdateMoveDetails(playerUnit.fables.Moves[selectedMoveIndex]);
        dialogBox.EnableDialogText(true);
        StartCoroutine(PerformPlayerMove());

        // Disable move selection, PP, and type
        dialogBox.EnableMoveSelector(false);
    }
    public void OnMoveButtonClick(int moveIndex)
    {
        selectedMoveIndex = moveIndex;
        ConfirmMoveSelection(moveIndex);
        DisableMoveDetails(); 
    }
}
