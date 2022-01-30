using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : StatefulTurnAI
{
    private int _currentTurnNumber;
    
    private Dictionary<int, IPlayedCard> _lastPlayedCard = new Dictionary<int, IPlayedCard>();
    private Dictionary<int, int> _focusTargets = new Dictionary<int, int>();
    private Dictionary<int, List<IPlayedCard>> _currentTurnPlayed = new Dictionary<int, List<IPlayedCard>>();
    
    public override void InitForBattle()
    {
        _lastPlayedCard = new Dictionary<int, IPlayedCard>();
        _focusTargets = new Dictionary<int, int>();
        _currentTurnPlayed = new Dictionary<int, List<IPlayedCard>>();
    }

    public IPlayedCard Play(int turnNumber, CardSelectionContext ctx)
    {
        var selected = Select(turnNumber, ctx);
        TrackState(selected);
        return selected;
    }
        
    private IPlayedCard Select(int turnNumber, CardSelectionContext ctx)
    {
        UpdateTurnTracking(turnNumber);
        
        return ctx
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(_ => _currentTurnPlayed.ValueOrMaybe(ctx.Member.Id).IsPresentAnd(cards => cards.Any(c => c.Card.Is(CardTag.Exclusive))), CardTag.Exclusive)
            .WithSelectedFocusCardIfApplicable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithFinalizedSmartCardSelection()
            .WithSelectedTargetsPlayedCard();
    }

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var focusTarget = GetFocusTarget(me, battleState);
        var lastPlayedCard = _lastPlayedCard.ValueOrMaybe(memberId).Map(c => c.Card.Type);
        
        return Select(battleState.TurnNumber, new CardSelectionContext(me, battleState, strategy, focusTarget, lastPlayedCard));
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
        => TrackState(card);
    
    private void TrackState(IPlayedCard card)
    {
        var memberId = card.MemberId();
        _lastPlayedCard[memberId] = card;
        if (!_currentTurnPlayed.ContainsKey(memberId))
            _currentTurnPlayed[memberId] = new List<IPlayedCard>();
        _currentTurnPlayed[memberId].Add(card);
        if (card.Card.Is(CardTag.Focus))
            _focusTargets[memberId] = card.PrimaryTargetId();
    }

    private Maybe<Member> GetFocusTarget(Member me, BattleState state)
    {
        var maybeTarget = _focusTargets.ValueOrMaybe(me.Id);
        if (maybeTarget.IsMissing)
            return Maybe<Member>.Missing();

        var maybeMember = state.Members.ValueOrMaybe(maybeTarget.Value);
        var memberStillValidFocusTarget = maybeMember
            .IsPresentAnd(m => state.GetPossibleConsciousTargets(me, Group.Opponent, Scope.One)
                .Any(t => t.Members.Length == 1 && t.Members.First().Equals(m)));
        
        if (!memberStillValidFocusTarget)
        {
            _focusTargets.Remove(me.Id);
            return Maybe<Member>.Missing();
        }

        return maybeMember;
    }
    
    private void UpdateTurnTracking(int currentTurnNumber)
    {
        if (currentTurnNumber == _currentTurnNumber)
            return;

        _currentTurnNumber = currentTurnNumber;
        _currentTurnPlayed = new Dictionary<int, List<IPlayedCard>>();
    }
}
