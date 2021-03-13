using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEnemyCardsPhases : OnMessage<BattleStateChanged, TurnStarted, CardResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private CardResolutionZone resolutionZone;
    
    private readonly AIStrategyGenerator _enemyStrategy = new AIStrategyGenerator(TeamType.Enemies);
    private AIStrategy _currentTurnStrategy;
    private BattleV2Phase _phase;
    
    private DictionaryWithDefault<int, int> _numberOfCardsPlayedThisTurn = new DictionaryWithDefault<int, int>(0);
    private List<(Member Member, Enemy Enemy)> _enemiesToActThisTurn = new List<(Member Member, Enemy Enemy)>();
    
    public void BeginPlayingAllHastyEnemyCards() => PlayNextHastyCard();
    private void PlayNextHastyCard()
    {
        _enemiesToActThisTurn
            .Where(x => x.Enemy.IsHasty)
            .FirstAsMaybe()
            .ExecuteIfPresentOrElse(
                Play, 
                () => resolutions.FinishResolvingAll(() => Message.Publish(new ResolutionsFinished(BattleV2Phase.HastyEnemyCards))));
    }
    
    public void BeginPlayingAllStandardEnemyCards() => PlayNextStandardCard();
    private void PlayNextStandardCard()
    {
        _enemiesToActThisTurn
            .Where(x => !x.Enemy.IsHasty)
            .FirstAsMaybe()
            .ExecuteIfPresentOrElse(
                Play, 
                () => resolutions.FinishResolvingAll(() => Message.Publish(new ResolutionsFinished(BattleV2Phase.EnemyCards))));
    }
    
    private void Play((Member Member, Enemy Enemy) e)
    {
        var card = e.Enemy.AI.Play(e.Member.Id, state, _currentTurnStrategy);
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
    protected override void Execute(TurnStarted msg)
    {
        _currentTurnStrategy = _enemyStrategy.Generate(state);
        _enemiesToActThisTurn = state.Enemies
            .Where(e => e.Member.IsConscious())
            .OrderBy(e => e.Enemy.PreferredTurnOrder)
            .ToList();
        _numberOfCardsPlayedThisTurn = new DictionaryWithDefault<int, int>(0);
    }
}
