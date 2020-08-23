using System.Linq;

public sealed class DumbAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        
        var card = playableCards.Random();
        var targets = card.ActionSequences.Select(action => 
            {
                var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
                var target = possibleTargets.Random();
                return target;
            });

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
