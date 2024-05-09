using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;


public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }
public enum BattleAction { Move, SwitchFable, UseItem, Run }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;

    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] Button backButton;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject faecharmSprite;


    public event Action<bool> OnBattleOver;
    private BattleAction playerAction;
    BattleState state;
    BattleState? prevState;
    int selectedMoveIndex;
    bool moveSelected = false;
    int selectedMemberIndex;
    bool switchAnimationInProgress = false;

    FableParty playerParty;
    FableParty trainerParty;
    Fables wildFables;

    bool isTrainerBattle = false;
    PlayerCon player;
    TrainerController trainer;

    int escapeAttempts;
    private Coroutine battleCoroutine;


    PartyMemberUI[] memberSlots;

    void Start()
    {
        memberSlots = GameObject.FindObjectsOfType<PartyMemberUI>();
        player = playerParty.GetComponent<PlayerCon>();
        foreach (PartyMemberUI member in memberSlots)
        {
            member.SetSelected(false);
        }

        backButton.onClick.AddListener(OnBackButtonClick);
    }


    public void StartTrainerBattle(FableParty playerParty, FableParty trainerParty)
    {
        this.playerParty = playerParty;
        List<Fables> fables = playerParty.Fables;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerCon>();
        trainer = trainerParty.GetComponent<TrainerController>();


        StartCoroutine(SetupBattle());
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
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            // wild fable battle
            playerUnit.Setup(playerParty.GetHealthyFable());
            enemyUnit.Setup(wildFables);

            dialogBox.SetMoveNames(playerUnit.fables.Moves);
            yield return dialogBox.TypeDialog($"A Wild {enemyUnit.fables.Base.FableName} appeared.");
        }
        else
        {
            // Trainer battle
            //Show trainer and player sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");

            //send out the first Fable of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyFables = trainerParty.GetHealthyFable();
            enemyUnit.Setup(enemyFables);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyFables.Base.FableName}");
            //send out the first Fable of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerFables = playerParty.GetHealthyFable();
            playerUnit.Setup(playerFables);
            yield return dialogBox.TypeDialog($"Go {playerFables.Base.FableName}!");
            dialogBox.SetMoveNames(playerUnit.fables.Moves);
        }
        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }


    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.fables.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }


    void ActionSelection()
    {
        DisableMoveDetails();
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

        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        yield return RunTurns(playerAction);
    }



    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        // Deduct PP for the move
        move.PP--;

        // Execute OnBeforeMove method of the source fable
        bool canRunMove = sourceUnit.fables.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.fables);
            yield break;
        }

        // Show status changes of the source fable
        yield return ShowStatusChanges(sourceUnit.fables);

        // Display a dialog indicating the move used
        yield return dialogBox.TypeDialog($"{sourceUnit.fables.Base.FableName} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation(sourceUnit, targetUnit.transform.position, move.Base.ParticleDuration);
        if (move.Base.ParticleEffectPrefab != null)
        {
            Vector3 particlePosition = targetUnit.transform.position - new Vector3(0f, 0.7f, 0f);

            GameObject particleEffectInstance = Instantiate(move.Base.ParticleEffectPrefab, particlePosition, move.Base.ParticleEffectPrefab.transform.rotation);

            ParticleSystem particleSystem = particleEffectInstance.GetComponent<ParticleSystem>();
            float particleDuration = particleSystem ? particleSystem.main.duration : 0f;

            Destroy(particleEffectInstance, particleDuration);
        }


        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit.fables, targetUnit.fables);
        }
        else
        {
            var damageDetails = targetUnit.fables.TakeDamage(move, sourceUnit.fables);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        // Check if the target fable has fainted
        if (targetUnit.fables.HP <= 0)
        {
            DisableMoveDetails();
            yield return dialogBox.TypeDialog($"{targetUnit.fables.Base.FableName} Fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(targetUnit);
        }

        // Execute OnAfterTurn method of the source fable
        sourceUnit.fables.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.fables);
        yield return sourceUnit.Hud.UpdateHP();

        // Check if the source fable has fainted
        if (sourceUnit.fables.HP <= 0)
        {
            DisableMoveDetails();
            yield return dialogBox.TypeDialog($"{sourceUnit.fables.Base.FableName} Fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(sourceUnit);
        }
        DisableMoveDetails();
    }



    IEnumerator RunMoveEffects(Move move, Fables source, Fables target)
    {
        var effects = move.Base.Effects;
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.ApplyBoosts(move.Base.Effects.Boosts); 
            }
            else
            {
                target.ApplyBoosts(move.Base.Effects.Boosts); 
            }
        }
        //Stat Condition
        if (effects.Status != ConditionID.None)
        {
            target.SetStatus(effects.Status);
        }

        if (source != null && source != null) 
        {
            yield return ShowStatusChanges(source);
        }
        if (target != null && target != null) 
        {
            yield return ShowStatusChanges(target);
        }
    }



    IEnumerator ShowStatusChanges(Fables fable)
    {
        Debug.Log("ShowStatusChanges coroutine started.");

        while (fable.StatusChanges.Count > 0)
        {
            var message = fable.StatusChanges.Dequeue();
            Debug.Log("Dequeued status change message: " + message);
            yield return dialogBox.TypeDialog(message);
        }

        Debug.Log("ShowStatusChanges coroutine finished.");
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
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextFable = trainerParty.GetHealthyFable();
                if (nextFable != null)
                {
                    StartCoroutine(SendNextTrainerFable(nextFable));
                }
                else
                {
                    BattleOver(true);
                }
            }
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
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        yield return new WaitForSeconds(0.1f);
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
    }

   public IEnumerator RunTurns(BattleAction playerAction)
{
    state = BattleState.RunningTurn;

    // Stop the current battle coroutine if it exists
    if (battleCoroutine != null)
    {
        StopCoroutine(battleCoroutine);
    }

    // Assign the current coroutine to the battleCoroutine variable
    battleCoroutine = StartCoroutine(ExecuteTurn(playerAction));

    yield return battleCoroutine;
}

private IEnumerator ExecuteTurn(BattleAction playerAction)
{
    if (playerAction == BattleAction.Move)
    {
            var selectedMove = playerUnit.fables.Moves[selectedMoveIndex];

        if (selectedMove.PP == 0)
        {
            yield return dialogBox.TypeDialog($"No PP left for {selectedMove.Base.Name}!");
            DisableMoveDetails();
            ActionSelection();
            yield break;
        }
        else if (selectedMove.PP < 0)
        {
            selectedMove.PP = 0;
        }

        // run the move since it has PP
        playerUnit.fables.CurrentMove = selectedMove;
        enemyUnit.fables.CurrentMove = enemyUnit.fables.GetRandomMove();

        int playerMovePriority = playerUnit.fables.CurrentMove.Base.Priority;
        int enemyMovePriority = enemyUnit.fables.CurrentMove.Base.Priority;

        // Check who goes first
        bool playerGoesFirst = true;
        if (enemyMovePriority > playerMovePriority)
        {
            playerGoesFirst = false;
        }
        else if (enemyMovePriority == playerMovePriority)
        {
            playerGoesFirst = playerUnit.fables.Speed >= enemyUnit.fables.Speed;
        }

        var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
        var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

        // First turn
        yield return RunMove(firstUnit, secondUnit, firstUnit.fables.CurrentMove);
        yield return RunAfterTurn(firstUnit);
        DisableMoveDetails();
        if (state == BattleState.BattleOver) yield break;

        // Check if both fables are still alive before proceeding to the second turn
        if (playerUnit.fables.HP > 0 && enemyUnit.fables.HP > 0)
        {
            // Second turn
            DisableMoveDetails();
            yield return RunMove(secondUnit, firstUnit, secondUnit.fables.CurrentMove);
            yield return RunAfterTurn(secondUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        // Check if any fable has fainted after turns
        if (playerUnit.fables.HP <= 0 || enemyUnit.fables.HP <= 0)
        {
            DisableMoveDetails();
            ResetBattle();
            yield break;
        }
    }
    else
    {
            if (playerAction == BattleAction.SwitchFable)
            {
                var selectedFable = playerParty.fables[selectedMemberIndex];
                state = BattleState.Busy;
                yield return SwitchFables(selectedFable);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowFaeCharm();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.fables.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);

            // Check if battle is over after enemy action
            if (state == BattleState.BattleOver) yield break;

            // Check if any fable has fainted after the enemy's turn
            if (playerUnit.fables.HP <= 0 || enemyUnit.fables.HP <= 0)
            {
                // Reset battle state
                DisableMoveDetails();
                ResetBattle();
                yield break;
            }
        }
        if (state != BattleState.BattleOver)
        {
            ActionSelection(); // Return to action selection
        }
        DisableMoveDetails();
    }





    private void ResetBattle()
    {
        if (battleCoroutine != null)
        {
            DisableMoveDetails();
            StopCoroutine(battleCoroutine);
        }

        state = BattleState.Start;
    }


    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection && !moveSelected)
        {
            HandlePlayerAction();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            prevState = state;
            OnFableButtonClicked(selectedMemberIndex);
        }
    }

    public void OpenBag()
    {
        dialogBox.EnableActionSelector(false);
        StartCoroutine(ThrowFaeCharm());
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
                var move = playerUnit.fables.Moves[selectedMoveIndex];
                if (move.PP == 0) return;
                dialogBox.EnableMoveSelector(false);
                dialogBox.EnableDialogText(true);
                StartCoroutine(RunTurns(BattleAction.Move));
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

        // Trigger the enemy's turn immediately after the player's fable switch
        if (state == BattleState.PartyScreen)
        {
            state = BattleState.RunningTurn; // Change state to RunningTurn for enemy's turn
            StartCoroutine(RunTurns(BattleAction.SwitchFable)); // Proceed with enemy's turn
            yield return new WaitUntil(() => state != BattleState.RunningTurn); // Wait until the enemy's turn finishes
        }

        switchAnimationInProgress = false;

        // If it's not a manual switch or the enemy's turn, continue with player's turn
        if (state != BattleState.RunningTurn)
        {
            ActionSelection();
        }
    }

    IEnumerator SendNextTrainerFable(Fables nextFable)
    {
        state = BattleState.Busy;

        enemyUnit.Setup(nextFable);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextFable.Base.FableName}");

        state = BattleState.RunningTurn;
        ActionSelection();
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
        state = BattleState.ActionSelection;
        ActionSelection();
        moveSelected = false;
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
        StartCoroutine(RunTurns(BattleAction.Move));

        dialogBox.EnableMoveSelector(false);
    }


    //buttons
    void OnButtonClickRun()
    {
        StartCoroutine(RunTurns(BattleAction.Run));
    }
    void OnMoveButtonClick(int moveIndex)
    {
        DisableMoveDetails();
        selectedMoveIndex = moveIndex;
        ConfirmMoveSelection(moveIndex);
    }


    public void OnFableButtonClicked(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < playerParty.Fables.Count)
        {
            var selectedMember = playerParty.fables[selectedIndex];

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

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchFable));
            }
            else
            {
                state = BattleState.Busy;
                selectedMemberIndex = selectedIndex;
                StartCoroutine(SwitchFables(selectedMember));
            }
        }
    }

    public void OnMoveSelectionButtonClick()
    {
        MoveSelection();
    }



    IEnumerator ThrowFaeCharm()
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't Steal the trainers fable!");
            state = BattleState.RunningTurn;
            ActionSelection();
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used FaeCharm!");
        var faecharmObj = Instantiate(faecharmSprite, playerUnit.transform.position - new Vector3(3, 0), Quaternion.identity);
        var faecharm = faecharmObj.GetComponent<SpriteRenderer>();

        //Animation Throw
        yield return faecharm.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return faecharm.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 1).WaitForCompletion();

        int shakeCount = TryToCatchFables(enemyUnit.fables);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return faecharm.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.fables.Base.FableName} was caught");
            yield return faecharm.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddFable(enemyUnit.fables);
            yield return dialogBox.TypeDialog($"{enemyUnit.fables.Base.FableName} has been added to your party");

            Destroy(faecharm);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            faecharm.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.fables.Base.FableName} broke free");
                Debug.Log("Fable broke free");
                yield break;
            }
            else
            {
                yield return dialogBox.TypeDialog($"Almost caught it!");
                Debug.Log("Almost caught it");
            }

            Destroy(faecharm);
            Debug.Log("Destroyed faecharm");

            // Only set the state to RunningTurn here, after the catch attempt
            state = BattleState.RunningTurn;
            Debug.Log("Set state to RunningTurn");
        }
    }

    int TryToCatchFables(Fables fable)
    {
        float a = (3 * fable.MaxHp - 2 * fable.HP) * fable.Base.CatchRate / (3 * fable.MaxHp);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            ++shakeCount;
        }

        return shakeCount;
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles");
            state = BattleState.RunningTurn;
            yield break;
        }
        ++escapeAttempts;

        int playerSpeed = playerUnit.fables.Speed;
        int enemySpeed = enemyUnit.fables.Speed;
        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 + escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape from the falbe it's too fast.");
                state = BattleState.RunningTurn;
            }
        }
    }

}