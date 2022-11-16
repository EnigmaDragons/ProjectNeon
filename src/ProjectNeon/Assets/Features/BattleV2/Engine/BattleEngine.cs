using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEngine : OnMessage<PlayerTurnConfirmed, StartOfTurnEffectsStatusResolved, EndOfTurnStatusEffectsResolved, 
    ResolutionsFinished, CardAndEffectsResolutionFinished, StartCardSetupRequested>
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
    [SerializeField] private CurrentCutscene cutscene;
    [SerializeField] private BattleCutscenePresenter battleCutscenePresenter;
    [SerializeField] private ConfirmPlayerTurnV2 confirm;
    [SerializeField] private BattleGlobalEffectCardsPhase globalEffectCardsPhases;

    private bool _triggeredBattleFinish;
    private bool _playerTurnConfirmed = false;

    private readonly BattleUnconsciousnessChecker _unconsciousness = new BattleUnconsciousnessChecker();
    
    private void Awake()
    {
        state.SetPhase(BattleV2Phase.NotBegun);
        cards.ClearAll();
        confirm.Init(resolutions);
    }
    
    public void Start()
    {
        if (setupOnStart)
            Setup();
    }
    
    public void Setup() => StartCoroutine(ExecuteSetupAsync());
    
    private IEnumerator ExecuteSetupAsync()
    {
        BeginPhase(BattleV2Phase.SetupCharacters);
        BattleLog.Write("Battle Setup Begun");
        _triggeredBattleFinish = false;
        yield return setup.ExecuteCharacters();
        if (cutscene.HasStartBattleCutscene)
        {
            BeginPhase(BattleV2Phase.Cutscene);
            yield return battleCutscenePresenter.Begin();
        }
        else
        {
            yield return ExecuteCardSetupAsync();
        }
    }
    
    private IEnumerator ExecuteCardSetupAsync()
    {
        BeginPhase(BattleV2Phase.SetupPlayerCards);
        yield return setup.ExecuteCards();
        BattleLog.Write("Battle Started");
        BeginStartOfTurn();
    }
    
    private void BeginStartOfTurn()
    {
        BattleLog.Write($"--------------  Turn {state.TurnNumber}  --------------");
        BeginPhase(BattleV2Phase.StartOfTurnEffects);
        if (state.TurnNumber == 1)
        {
            state.ApplyAllGlobalStartOfBattleEffects();
            Message.Publish(new RefreshCardsInHand());
        }
        state.StartTurn();
        _playerTurnConfirmed = false;
        Message.Publish(new TurnStarted());
        statusPhase.ProcessStartOfTurnEffects();
    }
    
    private void BeginGlobalEffectCardsPhase() =>
        ResolveBattleFinishedOrExecute(() =>
        {
            BeginPhase(BattleV2Phase.GlobalEffectCards);
            globalEffectCardsPhases.BeginPlayingAllGlobalEffectCards();
        });

    private void BeginHastyEnemiesPhase() =>
        ResolveBattleFinishedOrExecute(() =>
        {
            enemyCardsPhases.GenerateAiStrategy();
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
        if (_playerTurnConfirmed)
            yield break;
        
        _playerTurnConfirmed = true;
        resolutionZone.NotifyPlayerTurnEnded();
        while (!resolutions.IsDoneResolving)
            yield return new WaitForSeconds(0.1f);
        Message.Publish(new ResolutionsFinished(BattleV2Phase.PlayCards));
    }
    
    protected override void Execute(PlayerTurnConfirmed msg) => StartCoroutine(WaitForAllPlayerCardsToFinishResolving());
    protected override void Execute(StartOfTurnEffectsStatusResolved msg)
    { 
        Log.Info("StartOfTurnEffectsStatusResolved");
        if (state.TurnNumber == 1)
            BeginGlobalEffectCardsPhase();
        else
            BeginHastyEnemiesPhase();
    }

    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => BeginStartOfTurn();
    protected override void Execute(CardAndEffectsResolutionFinished msg) => ResolveBattleFinishedOrExecute(() => Message.Publish(new CheckForAutomaticTurnEnd()));
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ExecuteCardSetupAsync());

    protected override void Execute(ResolutionsFinished msg)
    {
        DevLog.Write($"Resolutions Finished {msg.Phase}");
        if (state.BattleIsOver())
            FinishBattle();
        else if (msg.Phase == BattleV2Phase.GlobalEffectCards)
            BeginHastyEnemiesPhase();
        else if (msg.Phase == BattleV2Phase.HastyEnemyCards)
            BeginPlayerCardsPhase();
        else if (msg.Phase == BattleV2Phase.PlayCards)
            StartCoroutine(TransitionToEnemyCardsPhase());
        else if (msg.Phase == BattleV2Phase.EnemyCards)
            StartCoroutine(WrapUpTurn());
    }

    private IEnumerator WrapUpTurn()
    {
        if (state.Phase == BattleV2Phase.Wrapup || state.Phase == BattleV2Phase.EndOfTurnEffects)
        {
            Log.Warn("Tried to Trigger Wrap Up Turn multiple times");
            yield break;
        }

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
        var startedOnFinish = false;
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
                if (startedOnFinish)
                    return;

                startedOnFinish = true;
                state.Heroes.Where(h => h.CurrentHp() < 1).ForEach(h => h.State.SetHp(1));
                state.Party.Heroes.ForEach(h => h.ApplyBattleEndEquipmentEffects(state.GetMemberByHero(h.Character), state));
                Message.Publish(new BattleFinished(TeamType.Party));
                state.Wrapup();
                BeginPhase(BattleV2Phase.Finished);
            });
        }
    }

    private static readonly Dictionary<BattleV2Phase, string> PhaseMessages = 
        Enum.GetValues(typeof(BattleV2Phase)).Cast<BattleV2Phase>()
            .ToDictionary(p => p, p => $"-- Phase - {p.ToString().WithSpaceBetweenWords()} --"); 
    
    private void BeginPhase(BattleV2Phase newPhase)
    {
        if (newPhase == state.Phase)
        {
            Log.Info($"Battle Phase {newPhase} already begun");
            return;
        }

        var finishedMessage = state.Phase != BattleV2Phase.NotBegun ? $"Finished {state.Phase} Phase -> " : string.Empty;
        var message = $"Phase - {finishedMessage}Beginning {newPhase} Phase";
        LogProcessStep(message);
        BattleLog.Write(PhaseMessages[newPhase]);
        state.CleanupExpiredMemberStates();
        state.SetPhase(newPhase);
    }
    
    private void LogProcessStep(string message)
    {
        if (logProcessSteps)
            DevLog.Write(message);
    }
}
