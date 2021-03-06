using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/GeneralAI")]
public class GeneralAI : StatefulTurnAI
{
    private Dictionary<int, IPlayedCard> _lastPlayedCard = new Dictionary<int, IPlayedCard>();

    public override void InitForBattle() => _lastPlayedCard = new Dictionary<int, IPlayedCard>();

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(battleState.Members[memberId], battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection(c => _lastPlayedCard.TryGetValue(memberId, out var card) && c.Equals(card.Card.Type) ? -99 : 0)
            .WithSelectedTargetsPlayedCard();
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        _lastPlayedCard[card.MemberId()] = card;
    }
}
