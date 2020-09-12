﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/CardResolutionZone")]
public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private List<IPlayedCard> moves = new List<IPlayedCard>();
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private CardPlayZone playerPlayArea;
    [SerializeField] private CardPlayZone physicalZone;
    [SerializeField] private CardPlayZone playedDiscardZone;
    [SerializeField] private bool isResolving;
    [SerializeField] private BattleState battleState;
    public IPlayedCard LastPlayed { get; set; }

    public void Init()
    {
        physicalZone.Clear();
        isResolving = false;
    }

    public bool HasMore => moves.Any();
    
    public void Add(IPlayedCard played)
    {
        moves.Add(played);
        physicalZone.PutOnBottom(played.Card);
        played.Member.Apply(m =>
        {
            m.Lose(played.Spent);
            m.Gain(played.Gained);
        });
        BattleLog.Write($"{played.Member.Name} Played {played.Card.Name} - Spent {played.Spent} - Gained {played.Gained}");
    }

    public void ExpirePlayedCards(Func<IPlayedCard, bool> condition)
    {
        var movesCopy = moves.ToArray();
        for (var i = movesCopy.Length - 1; i > -1; i--)
        {
            var played = movesCopy[i];
            if (!condition(played)) continue;
            
            BattleLog.Write($"Expired played card {played.Card.Name} by {played.Member.Name}");
            if (moves.Count > i)
                moves.RemoveAt(i);
            physicalZone.Take(i);
            if (played.Member.TeamType == TeamType.Party)
                playerHand.PutOnBottom(playerPlayArea.Take(i));
        }
    }
    
    public void RemoveLastPlayedCard()
    {
        if (moves.None() || isResolving) return;
        
        var played = moves.Last();
        
        BattleLog.Write($"Canceled playing {played.Card.Name}");
        Message.Publish(new PlayerCardCanceled());
        moves.RemoveAt(moves.Count - 1);
        var card = physicalZone.Take(physicalZone.Count - 1);
        playerPlayArea.Take(playerPlayArea.Count - 1);
        playerHand.PutOnBottom(card);
        
        played.Member.Apply(m => m.LoseResource(played.Gained.ResourceType, played.Gained.Amount));
        played.Member.Apply(m => m.GainResource(played.Spent.ResourceType, played.Spent.Amount));
    }

    public void BeginResolvingNext()
    {
        BattleLog.Write("Requested Resolve Next Card");
        isResolving = true;
        var move = moves[0];
        moves = moves.Skip(1).ToList();
        StartResolvingOneCard(move);
    }

    private void StartResolvingOneCard(IPlayedCard played)
    {
        BattleLog.Write($"Began resolving {played.Card.Name}");
        if (physicalZone.Count == 0)
            Log.Info($"Weird Physical Zone Draw bug.");
        else
            physicalZone.DrawOneCard();
        
        var card = played.Card;
        if (card.Owner.IsStunnedForCard())
        {
            BattleLog.Write($"{card.Owner.Name} was stunned, so {card.Name} does not resolve.");
            card.Owner.Apply(m => m.ApplyTemporaryAdditive(AdjustedStats.CreateIndefinite(new StatAddends().With(TemporalStatType.CardStun, -1), true)));
            WrapupCard(played, card);
            Message.Publish(new CardResolutionFinished());
        }
        else
        {
            played.Perform(battleState.GetSnapshot());
            WrapupCard(played, card);
        }
    }

    private void WrapupCard(IPlayedCard played, Card physicalCard)
    {
        LastPlayed = played;
        if (played.Member.TeamType.Equals(TeamType.Party))
            playedDiscardZone.PutOnBottom(physicalCard.RevertedToStandard());
        isResolving = moves.Any();
    }
}
