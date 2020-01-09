using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private List<PlayedCard> moves = new List<PlayedCard>();
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private CardPlayZone playerPlayArea;
    [SerializeField] private CardPlayZone physicalZone;
    [SerializeField] private CardPlayZone playedDiscardZone;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private GameEvent onCardResolved;
    public PlayedCard LastPlayed { get; set; }

    private bool _isResolving;

    public void Add(PlayedCard played)
    {
        if (_isResolving) return;
        
        moves.Add(played);
        physicalZone.PutOnBottom(played.Card);
        played.Member.Apply(m => m.Pay(played.Card.Cost));
        BattleLog.Write($"{played.Member.Name} Played {played.Card.name}");
    }

    public void RemoveLastPlayedCard()
    {
        if (_isResolving || moves.None()) return;
        
        var played = moves.Last();
        moves.RemoveAt(moves.Count - 1);
        var card = physicalZone.Take(physicalZone.Count - 1);
        playerPlayArea.Take(playerPlayArea.Count - 1);
        playerHand.PutOnBottom(card);
        played.Member.Apply(m => m.Refund(played.Card.Cost));
    }

    public void Resolve(MonoBehaviour host)
    {
        host.StartCoroutine(ResolveAll());
    }

    private IEnumerator ResolveAll()
    {
        _isResolving = true;
        foreach (var move in moves.ToList())
        {
            yield return ResolveOneCard(move);
            yield return new WaitForSeconds(1.1f);
        }

        _isResolving = false;
        onFinished.Publish();
    }
    
    private IEnumerator ResolveOneCard(PlayedCard played)
    {
        var card = physicalZone.DrawOneCard();
        BattleLog.Write($"Began resolving {played.Card.Name}");
        played.Perform();
        LastPlayed = played;
        if (played.Member.TeamType.Equals(TeamType.Party))
            playedDiscardZone.PutOnBottom(card);
        onCardResolved.Publish();
        yield break;
    }
}
