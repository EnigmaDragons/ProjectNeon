using System.Collections;
using UnityEngine;

public class BattleEngine : OnMessage<PlayerTurnConfirmed, ResolutionsFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private CardPlayZones cards;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattleCommandPhase commandPhase;
    [SerializeField] private BattleResolutionPhase resolutionPhase;
    [SerializeField] private BattleTurnWrapUp turnWrapUp;
    [SerializeField] private ShopCardPool cardPrizePool;
    [SerializeField] private bool logProcessSteps;
    [SerializeField] private bool setupOnStart;
    [SerializeField, ReadOnly] private BattleV2Phase phase = BattleV2Phase.NotBegun;

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
        BeginCommandPhase();
    }    
    
    private void BeginCommandPhase()
    {
        BeginPhase(BattleV2Phase.Command);
        state.StartTurn();
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

    protected override void Execute(ResolutionsFinished msg)
    {
        BeginPhase(BattleV2Phase.Wrapup);
        
        if (state.BattleIsOver())
            FinishBattle();
        else
        {
            turnWrapUp.Execute();
            BeginCommandPhase();
        }
    }

    private void FinishBattle()
    {
        if (state.PlayerLoses())
            Message.Publish(new BattleFinished(TeamType.Enemies));
        else if (state.PlayerWins())
        {
            var rewardPicker = new ShopSelectionPicker();
            state.SetRewardCards(rewardPicker.PickCards(state.Party, cardPrizePool, 2));
            Message.Publish(new BattleFinished(TeamType.Party));
        }

        state.Wrapup();
        BeginPhase(BattleV2Phase.Finished);
    }

    private void BeginPhase(BattleV2Phase newPhase)
    {
        var finishedMessage = phase != BattleV2Phase.NotBegun ? $"Finished {phase} Phase -> " : "";
        var message = $"{finishedMessage}Beginning {newPhase} Phase";
        LogProcessStep(message);
        phase = newPhase;
    }
    
    private void LogProcessStep(string message)
    {
        if (logProcessSteps)
            BattleLog.Write(message);
    }
    
    public enum BattleV2Phase
    {
        NotBegun = 0,
        Setup = 20,
        Command = 40,
        Resolution = 50,
        Wrapup = 60,
        Finished = 80
    }
}
