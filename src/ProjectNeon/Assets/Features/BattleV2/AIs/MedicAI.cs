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
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies 
            ? battleState.Enemies.Where(m => m.IsConscious()) 
            : battleState.Heroes.Where(m => m.IsConscious());
        
        var maybeCard = new Maybe<CardType>();
        IEnumerable<CardType> cardOptions = playableCards;
        // TODO: Dealing killing blow if possible with an attack card

        if (allies.Count() == 1 && cardOptions.Any(c => c.TypeDescription.Contains("Attack")))
            maybeCard = cardOptions.Where(c => c.TypeDescription.Contains("Attack"))
                .MostExpensive();
        
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
                return possibleTargets.MostDamaged();
            if (card.TypeDescription == "Defense" && card.Tags.Contains(CardTag.Shield))
            {
                if (possibleTargets.Any(x => !x.HasShield()))
                    return possibleTargets.Where(x => !x.HasShield())
                        .MostVulnerable();
                // Or, use shield to whomever could use the most
                return possibleTargets.OrderByDescending(x => x.TotalRemainingShieldCapacity()).First();
            }

            if (card.TypeDescription == "Attack")
                return strategy.AttackTargetFor(action);
            return possibleTargets.Random();
        });

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}
