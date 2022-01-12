using System;
using System.IO;
using System.Linq;

public static class AIHelpers
{
    public static CardTypeData[] GetPlayableCards(this BattleState s, int memberId, PartyAdventureState partyState, EnemySpecialCircumstanceCards specialCircumstanceCards)
    {
        var enemy = s.GetEnemyById(memberId);
        var me = s.Members[memberId];
        if (!me.IsConscious())
            throw new InvalidOperationException($"{me} is unconscious, but has been asked to play a card");

        var cards = enemy.Cards.Where(x => x != null).Concat(specialCircumstanceCards.AntiStealthCard);
        if (!enemy.DeckIsValid) 
            Log.Error($"{enemy.Name} has a deck with null cards");

        var opponentsAllStealthed = s.Heroes.All(m => m.IsStealthed());
        
        var playableCards = cards.Where(c => c.IsPlayableBy(me, partyState, 1) && c.HasAnyValidTargets(me, s)).Cast<CardTypeData>().ToArray();
        if (!opponentsAllStealthed && !playableCards.Any())
            Log.Info($"{me} has no playable cards in hand");
        return playableCards;
    }
}
