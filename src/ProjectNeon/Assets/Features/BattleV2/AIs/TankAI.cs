using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/TankAI")]
public sealed class TankAI : TurnAI
{
    private static readonly DictionaryWithDefault<CardTag, int> CardTypePriority = new DictionaryWithDefault<CardTag, int>(99)
    {
        { CardTag.Defense, 1 },
        { CardTag.Attack, 2}
    };
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies 
            ? battleState.Enemies.Where(m => m.IsConscious()) 
            : battleState.Heroes.Where(m => m.IsConscious());
        
        var maybeCard = new Maybe<CardTypeData>();
        IEnumerable<CardTypeData> cardOptions = playableCards;
        // TODO: Dealing killing blow if possible with an attack card

        var attackCards = cardOptions.Where(c => c.Is(CardTag.Attack)).ToList();
        if (allies.Count() == 1 && attackCards.Any())
            maybeCard = new Maybe<CardTypeData>(attackCards.MostExpensive());
        
        // Don't play a shield if all allies are already shielded
        if (allies.All(a => a.RemainingShieldCapacity() > a.MaxShield() * 0.7))
            cardOptions = cardOptions.Where(x => !x.Is(CardTag.Defense, CardTag.Shield));

        var card = maybeCard.IsPresent 
            ? maybeCard.Value 
            : cardOptions
                .ToArray()
                .Shuffled()
                .OrderByDescending(c => c.Cost.Amount)
                .ThenBy(c => CardTypePriority[c.Tags.First()]) // Maybe needs a better way to prioritze
                .First();
        
        var targets = card.ActionSequences.Select(action => 
        {
            var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
            if (card.Is(CardTag.Defense, CardTag.Shield))
            {
                if (possibleTargets.Any(x => !x.HasShield()))
                    return possibleTargets.Where(x => !x.HasShield())
                        .MostPowerful();
                // Or, use shield to whomever could use the most
                return possibleTargets.OrderByDescending(x => x.TotalRemainingShieldCapacity()).First();
            }

            if (card.Is(CardTag.Attack))
                return strategy.AttackTargetFor(action);
            return possibleTargets.Random();
        });

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }
}

