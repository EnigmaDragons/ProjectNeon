using System;
using System.IO;
using System.Linq;

public static class AIHelpers
{
    public static CardTypeData[] GetPlayableCards(this BattleState s, int memberId)
    {
        var enemy = s.GetEnemyById(memberId);
        var me = s.Members[memberId];
        if (!me.IsConscious())
            throw new InvalidOperationException($"{me} is unconscious, but has been asked to play a card");
        
        var playableCards = enemy.Deck.Cards.Where(c => c.IsPlayableBy(me)).Cast<CardTypeData>().ToArray();
        if (!playableCards.Any())
            throw new InvalidDataException($"{me} has no playable cards in hand");
        return playableCards;
    }
}
