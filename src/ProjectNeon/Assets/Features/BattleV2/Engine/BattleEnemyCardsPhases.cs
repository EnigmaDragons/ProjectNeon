using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEnemyCardsPhases : OnMessage<BattleStateChanged, CardResolutionFinished, UpdateAIStrategy, MemberUnconscious>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private CardType disabledCard;
    [SerializeField] private CardType antiStealthCard;
    [SerializeField] private CardType aiGlitchedCard;
    
    private readonly AIStrategyGenerator _enemyStrategy = new AIStrategyGenerator(TeamType.Enemies);
    private AIStrategy _currentTurnStrategy;
    private BattleV2Phase _phase;
    
    private DictionaryWithDefault<int, int> _numberOfCardsPlayedThisTurn = new DictionaryWithDefault<int, int>(0);
    private List<(Member Member, EnemyInstance Enemy)> _enemiesToActThisTurn = new List<(Member Member, EnemyInstance Enemy)>();

    public void GenerateAiStrategy()
    {
        DevLog.Write("Generated AI Strategy");
        _currentTurnStrategy = _enemyStrategy.Generate(state, new EnemySpecialCircumstanceCards(disabledCard, antiStealthCard, aiGlitchedCard));
        _enemiesToActThisTurn = state.Enemies
            .Where(e => e.Member.IsConscious())
            .OrderBy(e => e.Enemy.PreferredTurnOrder)
            .ThenBy(e => state.GetMaybeTransform(e.Member.Id).IsPresent ? state.GetMaybeTransform(e.Member.Id).Value.position.x : 0)
            .ToList();
        _numberOfCardsPlayedThisTurn = new DictionaryWithDefault<int, int>(0);
    }
    
    public void BeginPlayingAllHastyEnemyCards() => PlayNextHastyCard();
    private void PlayNextHastyCard()
    {
        DevLog.Info($"Enemies - Began Playing Next Hasty Card. {_enemiesToActThisTurn.Count(x => x.Enemy.IsHasty)} to act.");
        this.SafeCoroutineOrNothing(ExecuteAfterReactionsFinished(() =>
        {
            RemoveUnconsciousAndEscapedEnemiesFromActPool();
            Message.Publish(new UpdateAIStrategy());
            _enemiesToActThisTurn
                .Where(x => x.Enemy.IsHasty)
                .FirstAsMaybe()
                .ExecuteIfPresentOrElse(
                    Play,
                    () => this.SafeCoroutineOrNothing(WaitForResolutionsFinished(BattleV2Phase.HastyEnemyCards)));
        }));
    }
    
    public void BeginPlayingAllStandardEnemyCards() => PlayNextStandardCard();

    private void PlayNextStandardCard()
    {
        DevLog.Info($"Enemies - Began Playing Next Standard Card. {_enemiesToActThisTurn.Count(x => !x.Enemy.IsHasty)} to act.");
        this.SafeCoroutineOrNothing(ExecuteAfterReactionsFinished(() =>
        {
            RemoveUnconsciousAndEscapedEnemiesFromActPool();
            Message.Publish(new UpdateAIStrategy());
            _enemiesToActThisTurn
                .Where(x => !x.Enemy.IsHasty)
                .FirstAsMaybe()
                .ExecuteIfPresentOrElse(
                    Play,
                    () => this.SafeCoroutineOrNothing(WaitForResolutionsFinished(BattleV2Phase.EnemyCards)));
        }));
}
    
    private IEnumerator WaitForResolutionsFinished(BattleV2Phase phase)
    {
        while (!resolutions.IsDoneResolving)
            yield return new WaitForSeconds(0.1f);
        Message.Publish(new ResolutionsFinished(phase));
    }

    private IEnumerator ExecuteAfterReactionsFinished(Action onFinished)
    {
        while (!resolutions.IsDoneResolving || state.Reactions.Any)
            yield return new WaitForSeconds(0.1f);
        onFinished();
    }
    
    private void RemoveUnconsciousAndEscapedEnemiesFromActPool() 
        => _enemiesToActThisTurn.RemoveAll(e => e.Member.IsUnconscious() || !state.Members.ContainsKey(e.Member.Id));

    private void Play((Member Member, EnemyInstance Enemy) e)
    {
        RemoveUnconsciousAndEscapedEnemiesFromActPool();
        if (_currentTurnStrategy.ShouldRegenerate)
        {
            var previous = _currentTurnStrategy;
            _currentTurnStrategy = _enemyStrategy.Generate(state, new EnemySpecialCircumstanceCards(disabledCard, antiStealthCard, aiGlitchedCard));
            previous.SelectedNonStackingTargets.ForEach(ns => ns.Value.ForEach(t => _currentTurnStrategy.RecordNonStackingTarget(ns.Key, t)));
        }

        var card = e.Enemy.AI.Play(e.Member.Id, state, _currentTurnStrategy);
        Message.Publish(new EnemyCardPlayed(card));
        _numberOfCardsPlayedThisTurn[card.MemberId()] = _numberOfCardsPlayedThisTurn[card.MemberId()] + 1;
        if (_numberOfCardsPlayedThisTurn[card.MemberId()] == e.Member.State.ExtraCardPlays())
            _enemiesToActThisTurn.Remove(e);
        resolutionZone.PlayImmediately(card);
    }

    private void PlayNextCardInPhase()
    {
        if (_phase == BattleV2Phase.HastyEnemyCards)
            PlayNextHastyCard();
        if (_phase == BattleV2Phase.EnemyCards)
            PlayNextStandardCard();
    }

    protected override void Execute(BattleStateChanged msg) => _phase = state.Phase;
    protected override void Execute(CardResolutionFinished msg) => PlayNextCardInPhase();
    
    protected override void Execute(UpdateAIStrategy msg)
    {
        _currentTurnStrategy = _enemyStrategy.Update(_currentTurnStrategy, state);
    }

    protected override void Execute(MemberUnconscious msg)
    {
        if (msg.Member.TeamType == TeamType.Enemies)
            Execute(new UpdateAIStrategy());
    }
}
