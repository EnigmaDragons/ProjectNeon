using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleEngine : OnMessage<PlayerTurnConfirmed, StartOfTurnEffectsStatusResolved, EndOfTurnStatusEffectsResolved, ResolutionsFinished, CardResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones cards;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattlePlayerCardsPhase playerCardsPhase;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private BattleEnemyCardsPhases enemyCardsPhases;
    [SerializeField] private BattleTurnWrapUp turnWrapUp;
    [SerializeField] private BattleStatusEffects statusPhase;
    [SerializeField] private BattleConclusion conclusion;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private bool logProcessSteps;
    [SerializeField] private bool setupOnStart;

    private bool _triggeredBattleFinish;

    private readonly BattleUnconsciousnessChecker _unconsciousness = new BattleUnconsciousnessChecker();
    
    private void Awake()
    {
        cards.ClearAll();
    }
    
    public void Start()
    {
        if (setupOnStart)
            Setup();
    }
    
    public void Setup() => StartCoroutine(ExecuteSetupAsync());
    
    private IEnumerator ExecuteSetupAsync()
    {
        BeginPhase(BattleV2Phase.Setup);
        _triggeredBattleFinish = false;
        yield return setup.Execute();
        BattleLog.Write("Battle Started");
        BeginStartOfTurn();
    }

    private void BeginStartOfTurn()
    {
        BattleLog.Write($"---------------------------------------------");
        BattleLog.Write($"Starting Turn {state.TurnNumber}");
        BeginPhase(BattleV2Phase.StartOfTurnEffects);
        state.StartTurn();
        Message.Publish(new TurnStarted());
        statusPhase.ProcessStartOfTurnEffects();
    }

    private void BeginHastyEnemiesPhase() =>
        ResolveBattleFinishedOrExecute(() =>
        {
            BeginPhase(BattleV2Phase.HastyEnemyCards);
            enemyCardsPhases.BeginPlayingAllHastyEnemyCards();
        });
    
    private void BeginPlayerCardsPhase() =>
        ResolveBattleFinishedOrExecute(() =>
        {
            BeginPhase(BattleV2Phase.PlayCards);
            playerCardsPhase.Begin();
        });

    private void ResolveBattleFinishedOrExecute(Action action)
    {
        _unconsciousness.ProcessUnconsciousMembers(state);
        _unconsciousness.ProcessRevivedMembers(state);
        if (state.BattleIsOver())
            FinishBattle();
        else
            action();
    }

    private IEnumerator TransitionToEnemyCardsPhase()
    {
        yield return playerCardsPhase.Wrapup();
        ResolveBattleFinishedOrExecute(() =>
        {
            BeginPhase(BattleV2Phase.EnemyCards);
            enemyCardsPhases.BeginPlayingAllStandardEnemyCards();
        });
    }

    private IEnumerator WaitForAllPlayerCardsToFinishResolving()
    {
        resolutionZone.NotifyPlayerTurnEnded();
        while (!resolutions.IsDoneResolving)
            yield return new WaitForSeconds(0.1f);
        Message.Publish(new ResolutionsFinished(BattleV2Phase.PlayCards));
    }
    
    protected override void Execute(PlayerTurnConfirmed msg) => StartCoroutine(WaitForAllPlayerCardsToFinishResolving());
    protected override void Execute(StartOfTurnEffectsStatusResolved msg) => BeginHastyEnemiesPhase();
    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => BeginStartOfTurn();
    protected override void Execute(CardResolutionFinished msg) => ResolveBattleFinishedOrExecute(() => { });

    protected override void Execute(ResolutionsFinished msg)
    {
        DevLog.Write($"Resolutions Finished {msg.Phase}");
        if (state.BattleIsOver())
            FinishBattle();
        else if (msg.Phase == BattleV2Phase.HastyEnemyCards)
            BeginPlayerCardsPhase();
        else if (msg.Phase == BattleV2Phase.PlayCards)
            StartCoroutine(TransitionToEnemyCardsPhase());
        else
            StartCoroutine(WrapUpTurn());
    }


    private IEnumerator WrapUpTurn()
    {
        BeginPhase(BattleV2Phase.Wrapup);
        yield return turnWrapUp.Execute();
        BeginPhase(BattleV2Phase.EndOfTurnEffects);
        statusPhase.ProcessEndOfTurnEffects();
    }

    private void FinishBattle()
    {
        if (_triggeredBattleFinish)
            return;
        
        _triggeredBattleFinish = true;
        if (state.PlayerLoses())
        {
            Message.Publish(new BattleFinished(TeamType.Enemies));
            state.Wrapup();
            BeginPhase(BattleV2Phase.Finished);
        }
        else if (state.PlayerWins())
        {
            conclusion.GrantVictoryRewardsAndThen(() =>
            {
                state.Heroes.Where(h => h.CurrentHp() < 1).ForEach(h => h.State.SetHp(1));
                Message.Publish(new BattleFinished(TeamType.Party));
                state.Wrapup();
                BeginPhase(BattleV2Phase.Finished);
            });
        }
    }

    private void BeginPhase(BattleV2Phase newPhase)
    {
        if (newPhase == state.Phase)
            Log.Error($"Phase - Should not attempt to transition to {newPhase} when that's already the active phase.");
        
        var finishedMessage = state.Phase != BattleV2Phase.NotBegun ? $"Finished {state.Phase} Phase -> " : "";
        var message = $"Phase - {finishedMessage}Beginning {newPhase} Phase";
        LogProcessStep(message);
        state.CleanupExpiredMemberStates();
        state.SetPhase(newPhase);
    }
    
    private void LogProcessStep(string message)
    {
        if (logProcessSteps)
            DevLog.Write(message);
    }
}
