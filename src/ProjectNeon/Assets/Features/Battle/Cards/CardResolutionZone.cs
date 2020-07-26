using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private List<IPlayedCard> moves = new List<IPlayedCard>();
    [SerializeField] private CardPlayZone playerHand;
    [SerializeField] private CardPlayZone playerPlayArea;
    [SerializeField] private CardPlayZone physicalZone;
    [SerializeField] private CardPlayZone playedDiscardZone;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private GameEvent onCardResolved;
    public IPlayedCard LastPlayed { get; set; }

    public bool HasMore => moves.Any();
    
    public void Add(IPlayedCard played)
    {
        moves.Add(played);
        physicalZone.PutOnBottom(played.Card);
        played.Member.Apply(m => m.LoseResource(played.Spent.ResourceType.Name, played.Spent.Amount));
        BattleLog.Write($"{played.Member.Name} Played {played.Card.name} - {played.Spent}");
    }

    public void RemoveLastPlayedCard()
    {
        if (moves.None()) return;
        
        var played = moves.Last();
        BattleLog.Write($"Canceled playing {played.Card.Name}");
        moves.RemoveAt(moves.Count - 1);
        var card = physicalZone.Take(physicalZone.Count - 1);
        playerPlayArea.Take(playerPlayArea.Count - 1);
        playerHand.PutOnBottom(card);
        played.Member.Apply(m => m.GainResource(played.Spent.ResourceType.Name, played.Spent.Amount));
    }

    public void Resolve(MonoBehaviour host, float delay)
    {
        BattleLog.Write($"Card Resolution Began");
        host.StartCoroutine(ResolveAll(delay));
    }

    public IEnumerator ResolveNext(float delay)
    {
        BattleLog.Write($"Num Cards To Resolve: {moves.Count}");

        var move = moves[0];
        moves = moves.Skip(1).ToList();
        yield return new WaitForSeconds(delay);
        yield return ResolveOneCard(move);
    }

    private IEnumerator ResolveAll(float delay)
    {
        BattleLog.Write($"Num Cards To Resolve: {moves.Count}");
        yield return new WaitForSeconds(delay);
        foreach (var move in moves.ToList())
        {
            yield return ResolveOneCard(move);
            yield return new WaitForSeconds(1.1f);
        }

        moves.Clear();
        onFinished.Publish();
    }
    
    private IEnumerator ResolveOneCard(IPlayedCard played)
    {
        BattleLog.Write($"Began resolving {played.Card.Name}");
        if (physicalZone.Count == 0)
        {
            Debug.Log($"Weird Physical Zone Draw bug.");
            yield break;
        }
        var card = physicalZone.DrawOneCard();
        played.Perform();
        LastPlayed = played;
        if (played.Member.TeamType.Equals(TeamType.Party))
            playedDiscardZone.PutOnBottom(card);
        onCardResolved.Publish();
    }
}
