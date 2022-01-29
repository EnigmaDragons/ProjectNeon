using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : StatefulTurnAI
{
    private Dictionary<int, IPlayedCard> _lastPlayedCard = new Dictionary<int, IPlayedCard>();
    private Dictionary<int, int> _focusTargets = new Dictionary<int, int>();

    public override void InitForBattle()
    {
        _lastPlayedCard = new Dictionary<int, IPlayedCard>();
        _focusTargets = new Dictionary<int, int>();
    }

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var focusTarget = GetFocusTarget(me, battleState);
        var lastPlayedCard = _lastPlayedCard.ValueOrMaybe(memberId).Map(c => c.Card.Type);
        return new CardSelectionContext(me, battleState, strategy, focusTarget, lastPlayedCard)
            .WithCommonSenseSelections()
            .WithSelectedFocusCardIfApplicable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithFinalizedSmartCardSelection()
            .WithSelectedTargetsPlayedCard();
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        var memberId = card.MemberId();
        _lastPlayedCard[memberId] = card;
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
}
