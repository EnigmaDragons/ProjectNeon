using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleEngine : OnMessage<PlayerTurnConfirmed, StartOfTurnEffectsStatusResolved, EndOfTurnStatusEffectsResolved, ResolutionsFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones cards;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattlePlayerCardsPhase commandPhase;
    [SerializeField] private BattleResolutionPhase resolutionPhase;
    [SerializeField] private BattleEnemyCardsPhases enemyCardsPhases;
    [SerializeField] private BattleTurnWrapUp turnWrapUp;
    [SerializeField] private BattleStatusEffects statusPhase;
    [SerializeField] private BattleConclusion conclusion;
    [SerializeField] private bool logProcessSteps;
    [SerializeField] private bool setupOnStart;

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
        yield return setup.Execute();
        BattleLog.Write("Battle Started");
        BeginStartOfTurn();
    }

    private void BeginStartOfTurn()
    {
        BattleLog.Write($"Starting Turn {state.TurnNumber}");
        BeginPhase(BattleV2Phase.StartOfTurnEffects);
        state.StartTurn();
        Message.Publish(new TurnStarted());
        statusPhase.ProcessStartOfTurnEffects();
    }

    private void BeginHastyEnemiesPhase()
    {
        BeginPhase(BattleV2Phase.HastyEnemyCards);
        enemyCardsPhases.BeginPlayingAllHastyEnemyCards();
    }
    
    private void BeginPlayerCardsPhase()
    {
        BeginPhase(BattleV2Phase.PlayCards);
        _unconsciousness.ProcessUnconsciousMembers(state);
        _unconsciousness.ProcessRevivedMembers(state);
        if (state.BattleIsOver())
            FinishBattle();
        else
            commandPhase.Begin();
    }

    private IEnumerator TransitionToEnemyCardsPhase()
    {
        yield return commandPhase.Wrapup();
        BeginPhase(BattleV2Phase.EnemyCards);
        yield return resolutionPhase.Begin();
        enemyCardsPhases.BeginPlayingAllStandardEnemyCards();
    }
    
    protected override void Execute(PlayerTurnConfirmed msg) => StartCoroutine(TransitionToEnemyCardsPhase());
    protected override void Execute(StartOfTurnEffectsStatusResolved msg) => BeginHastyEnemiesPhase();
    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => BeginStartOfTurn();

    protected override void Execute(ResolutionsFinished msg)
    {
        DevLog.Write($"Resolutions Finished {msg.Phase}");
        if (msg.Phase == BattleV2Phase.HastyEnemyCards)
            BeginPlayerCardsPhase();
        else if (msg.Phase == BattleV2Phase.PlayCards)
            StartCoroutine(TransitionToEnemyCardsPhase());
        else
        {
            if (state.BattleIsOver())
                FinishBattle();
            else
                StartCoroutine(WrapUpTurn());
        }
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
            Log.Error($"Should not attempt to transition to {newPhase} when that's already the active phase.");
        
        var finishedMessage = state.Phase != BattleV2Phase.NotBegun ? $"Finished {state.Phase} Phase -> " : "";
        var message = $"{finishedMessage}Beginning {newPhase} Phase";
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
