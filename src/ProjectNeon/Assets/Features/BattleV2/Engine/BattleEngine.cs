using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleEngine : OnMessage<PlayerTurnConfirmed, StartOfTurnEffectsStatusResolved, EndOfTurnStatusEffectsResolved, ResolutionsFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones cards;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattleCommandPhase commandPhase;
    [SerializeField] private BattleResolutionPhase resolutionPhase;
    [SerializeField] private BattleTurnWrapUp turnWrapUp;
    [SerializeField] private BattleStatusEffects statusPhase;
    [SerializeField] private BattleConclusion conclusion;
    [SerializeField] private bool logProcessSteps;
    [SerializeField] private bool setupOnStart;
    [SerializeField, ReadOnly] private BattleV2Phase phase = BattleV2Phase.NotBegun;

    private int _turnNumber;
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
        BattleLog.Write($"Starting Turn {++_turnNumber}");
        BeginPhase(BattleV2Phase.StartOfTurnEffects);
        state.StartTurn();
        statusPhase.ProcessStartOfTurnEffects();
    }
    
    private void BeginCommandPhase()
    {
        BeginPhase(BattleV2Phase.PlayCards);
        _unconsciousness.ProcessUnconsciousMembers(state);
        _unconsciousness.ProcessRevivedMembers(state);
        if (state.BattleIsOver())
            FinishBattle();
        else
        {
            commandPhase.Begin();
            Message.Publish(new TurnStarted());
        }
    }

    private IEnumerator TransitionToResolutionPhase()
    {
        yield return commandPhase.Wrapup();
        BeginPhase(BattleV2Phase.Resolution);
        yield return resolutionPhase.Begin();
    }
    
    protected override void Execute(PlayerTurnConfirmed msg)
    {
        StartCoroutine(TransitionToResolutionPhase());
    }

    protected override void Execute(StartOfTurnEffectsStatusResolved msg)
    {
        BeginCommandPhase();
    }

    protected override void Execute(EndOfTurnStatusEffectsResolved msg)
    {
        BeginStartOfTurn();
    }

    protected override void Execute(ResolutionsFinished msg)
    {
        BeginPhase(BattleV2Phase.Wrapup);
        
        if (state.BattleIsOver())
            FinishBattle();
        else
        {
            turnWrapUp.Execute();
            BeginPhase(BattleV2Phase.EndOfTurnEffects);
            statusPhase.ProcessEndOfTurnEffects();
        }
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
        var finishedMessage = phase != BattleV2Phase.NotBegun ? $"Finished {phase} Phase -> " : "";
        var message = $"{finishedMessage}Beginning {newPhase} Phase";
        LogProcessStep(message);
        phase = newPhase;
        state.SetPhase(newPhase);
    }
    
    private void LogProcessStep(string message)
    {
        if (logProcessSteps)
            DevLog.Write(message);
    }
}
