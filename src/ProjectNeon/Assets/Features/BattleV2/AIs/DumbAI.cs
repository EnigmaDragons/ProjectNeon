using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public sealed class DumbAI : TurnAI
{
    [SerializeField] private BattleState battleState;

    public DumbAI Init(BattleState state)
    {
        battleState = state;
        return this;
    }

    public override IPlayedCard Play(int memberId)
    {
        var enemy = battleState.GetEnemyById(memberId);
        var me = battleState.Members[memberId];
        if (!me.IsConscious())
            throw new InvalidOperationException($"{me} is unconscious, but has been asked to play a card");
        
        var playableCards = enemy.Deck.Cards.Where(c => c.IsPlayableBy(me)).ToList();
        if (!playableCards.Any())
            throw new InvalidDataException($"{me} has no playable cards in hand");
        
        var card = playableCards.Random();
        var targets = new List<Target>();
        card.ActionSequences.ForEach(
            action =>
            {
                var possibleTargets = battleState.GetPossibleConsciousTargets(me, action.Group, action.Scope);
                var target = possibleTargets.Random();
                targets.Add(target);
            }
        );

        // TODO: We probably need to have Real Enemy Cards, Eventually
        var cardInstance = card.CreateInstance(battleState.GetNextCardId(), me);
        
        if (targets.Count < card.Actions.Length)
            Debug.LogError($"Only selected {targets.Count} for {card.Actions.Length} actions");
        
        return new PlayedCardV2(me, targets.ToArray(), cardInstance);
    }

}
