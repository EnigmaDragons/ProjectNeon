using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Tech CEO")]
public class TechCeoAI : StatefulTurnAI
{
    private string _lastCardPlayed = "";
    private int _characterToStun = -1;
    
    public override void InitForBattle() {}

    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithPhases()
            .WithRemovedCardByNameIfPresent(_lastCardPlayed)
            .WithSelectedTargetsPlayedCard(target => target.Members.All(member => !member.IsDisabled() && member.Id != _characterToStun));
    }

    protected override void TrackState(IPlayedCard card, BattleState state, AIStrategy strategy)
    {
        _lastCardPlayed = card.Card.Name;
        if (card.Spent.Amount == 1)
            _characterToStun = card.Targets[1].Members[0].Id;
    }
}