using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Controller")]
public class ControllerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {        
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = battleState.GetConsciousAllies(me);
        var enemies = battleState.GetConsciousEnemies(me);

        var maybeCard = new Maybe<CardTypeData>();
        IEnumerable<CardTypeData> cardOptions = playableCards;
        
        // Go Ham if Alone
        if (allies.Count() == 1 && cardOptions.Any(c => c.Tags.Contains(CardTag.Attack)))
            maybeCard = new Maybe<CardTypeData>(cardOptions.Where(c => c.Tags.Contains(CardTag.Attack)).MostExpensive());
        // Play Ultimate
        else if (cardOptions.Any(c => c.Tags.Contains(CardTag.Ultimate)))
            maybeCard = new Maybe<CardTypeData>(cardOptions.Where(c => c.Tags.Contains(CardTag.Ultimate)).MostExpensive());

        // Don't Hit Enemy Shields if the play isn't very effective
        if (enemies.Sum(e => e.CurrentShield()) < 15)
            cardOptions = cardOptions.Where(c => !c.Tags.Contains(CardTag.Shield));
        
        // Pick any other highest-cost card
        var card = maybeCard.IsPresent 
            ? maybeCard.Value 
            : cardOptions
                .ToArray()
                .Shuffled()
                .OrderByDescending(c => c.Cost.Amount)
                .First();
        
        var targets = card.ActionSequences.Select(action => 
        {
            var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
            if (card.Tags.Contains(CardTag.Stun))
                return possibleTargets.MostPowerful();
            if (card.Tags.Contains(CardTag.Attack))
                return strategy.AttackTargetFor(action);
            return possibleTargets.Random();
        });

        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);  
    }
}
