using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] Button backButton;
    [SerializeField] PartyScreen partyScreen;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int selectedMoveIndex;
    bool moveSelected = false;
    int selectedMemberIndex;
    bool switchAnimationInProgress = false;

    FableParty playerParty;
    Fables wildFables;

    PartyMemberUI[] memberSlots;

    void Start()
    {
        memberSlots = GameObject.FindObjectsOfType<PartyMemberUI>();
        foreach (PartyMemberUI member in memberSlots)
        {
            member.SetSelected(false);
        }

        backButton.onClick.AddListener(OnBackButtonClick);
    }


    public void StartBattle(FableParty playerParty, Fables wildFables)
    {
        this.playerParty = playerParty;
        List<Fables> fables = playerParty.Fables;
        this.wildFables = wildFables;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyFable());
        enemyUnit.Setup(wildFables);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.fables.Moves);

        yield return dialogBox.TypeDialog($"A Wild {enemyUnit.fables.Base.FableName} appeared.");

        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
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

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.fables.Moves[selectedMoveIndex];
        yield return RunMove(playerUnit, enemyUnit, move);

        // If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
        dialogBox.EnableMoveDetails(false);
    }


    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.fables.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // If the battle stat was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
    }


    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;

        yield return dialogBox.TypeDialog($"{sourceUnit.fables.Base.FableName} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();
        var damageDetails = targetUnit.fables.TakeDamage(move, sourceUnit.fables);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.fables.Base.FableName} Fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
    }


    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextFable = playerParty.GetHealthyFable();
            if (nextFable != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
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
        if (state == BattleState.ActionSelection)
        {
            HandlePlayerAction();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            OnFableButtonClicked(selectedMemberIndex);
        }
    }

    void HandleMoveSelection()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            Vector2 inputPosition = (Input.touchCount > 0) ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            selectedMoveIndex = dialogBox.CalculateSelectedMoveIndex(inputPosition);

            if (!moveSelected)
            {
                dialogBox.UpdateMoveSelection(selectedMoveIndex, playerUnit.fables.Moves[selectedMoveIndex]);
                dialogBox.EnableMoveDetails(true);
                moveSelected = true;
            }
            else
            {
                ConfirmMoveSelection(selectedMoveIndex);
                DisableMoveDetails();
            }
        }
    }

    IEnumerator SwitchFables(Fables newFable)
    {
        if (switchAnimationInProgress)
        {
            yield return new WaitUntil(() => !switchAnimationInProgress);
        }
        switchAnimationInProgress = true;

        if (playerUnit.fables.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.fables.Base.FableName}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newFable);
        dialogBox.SetMoveNames(newFable.Moves);
        yield return dialogBox.TypeDialog($"Go {newFable.Base.FableName}!");
        state = BattleState.Busy;
        yield return StartCoroutine(EnemyMove());
        switchAnimationInProgress = false;
        if (state != BattleState.PartyScreen)
        {
            dialogBox.EnableActionSelector(true);
        }
    }


    void DeselectAllPartyMembers()
    {
        foreach (PartyMemberUI member in memberSlots)
        {
            member.SetSelected(false);
        }
    }

    public void OnBackButtonClick()
    {
        ResetBattleState();
    }

    void ResetBattleState()
    {
        state = BattleState.ActionSelection;
        moveSelected = false;
        dialogBox.EnableActionSelector(true);
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableDialogText(true);
        dialogBox.EnableMoveDetails(false);
    }

    public void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Fables);
        partyScreen.gameObject.SetActive(true);
        selectedMemberIndex = -1;
        partyScreen.DeselectAllPartyMembers();

        dialogBox.EnableActionSelector(false);
    }


    void ConfirmMoveSelection(int selectedMoveIndex)
    {
        moveSelected = false;
        dialogBox.UpdateMoveDetails(playerUnit.fables.Moves[selectedMoveIndex]);
        dialogBox.EnableDialogText(true);
        StartCoroutine(PlayerMove());

        dialogBox.EnableMoveSelector(false);
    }
    //buttons
    void OnButtonClickRun()
    {
        print("The player has fled the scene!!");
    }
    void OnMoveButtonClick(int moveIndex)
    {
        selectedMoveIndex = moveIndex;
        ConfirmMoveSelection(moveIndex);
        DisableMoveDetails();
    }


    public void OnFableButtonClicked(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < playerParty.Fables.Count)
        {
            var selectedMember = playerParty.Fables[selectedIndex];

            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a Fainted Fable");
                return;
            }

            if (selectedMember == playerUnit.fables)
            {
                partyScreen.SetMessageText("You can't switch with the same Fable");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            selectedMemberIndex = selectedIndex; // Update selected member index
            StartCoroutine(SwitchFables(selectedMember));
        }
    }

   public void OnMoveSelectionButtonClick()
    {
        MoveSelection();
    }

}