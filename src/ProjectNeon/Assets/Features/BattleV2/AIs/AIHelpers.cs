using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class AIHelpers
{
    public static CardType[] GetPlayableCards(this BattleState s, int memberId)
    {
        var enemy = s.GetEnemyById(memberId);
        var me = s.Members[memberId];
        if (!me.IsConscious())
            throw new InvalidOperationException($"{me} is unconscious, but has been asked to play a card");
        
        var playableCards = enemy.Deck.Cards.Where(c => c.IsPlayableBy(me)).ToArray();
        if (!playableCards.Any())
            throw new InvalidDataException($"{me} has no playable cards in hand");
        return playableCards;
    }

    public static CardType MostExpensive(this IEnumerable<CardType> cards) => cards
        .ToArray()
        .Shuffled()
        .OrderByDescending(x => x.Cost.Amount)
        .First();

    // TODO: In the future, factor in armor/resist/vulnerable
    public static Target MostVulnerable(this IEnumerable<Target> targets) => targets
        .ToArray()
        .Shuffled()
        .OrderBy(t => t.TotalHpAndShields())
        .First();
}
