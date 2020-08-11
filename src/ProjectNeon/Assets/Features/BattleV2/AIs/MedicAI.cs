using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MedicAI")]
public sealed class MedicAI : TurnAI
{
    private static readonly DictionaryWithDefault<string, int> CardTypePriority = new DictionaryWithDefault<string, int>(99)
    {
        { "Healing", 1 },
        { "Defense", 2 },
    };
    
    public override IPlayedCard Play(int memberId, BattleState battleState)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies ? battleState.Enemies : battleState.Heroes;
        
        var maybeCard = new Maybe<CardType>();
        IEnumerable<CardType> cardOptions = playableCards;
        // TODO: Dealing killing blow if possible with an attack card
        
        if (allies.All(a => a.CurrentHp() >= a.MaxHp() * 0.9))
            cardOptions = cardOptions.Where(x => x.TypeDescription != "Healing");

        if (allies.All(a => a.RemainingShieldCapacity() > a.MaxShield() * 0.7))
            cardOptions = cardOptions.Where(x => !(x.TypeDescription == "Defense" && x.Tags.Contains(CardTag.Shield)));

        var card = maybeCard.IsPresent 
            ? maybeCard.Value 
            : cardOptions
                .ToArray()
                .Shuffled()
                .OrderByDescending(c => c.Cost.Amount)
                .ThenBy(c => CardTypePriority[c.TypeDescription])
                .First();
        
        var targets = card.ActionSequences.Select(action => 
        {
            var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
            if (card.TypeDescription == "Healing")
                return possibleTargets.OrderByDescending(x => x.TotalMissingHp()).First();
            if (card.TypeDescription == "Defense" && card.Tags.Contains(CardTag.Shield))
                return possibleTargets.OrderByDescending(x => x.TotalRemainingShieldCapacity()).First();
            if (card.TypeDescription == "Attack")
                return possibleTargets.MostVulnerable();
            return possibleTargets.Random();
        });

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
