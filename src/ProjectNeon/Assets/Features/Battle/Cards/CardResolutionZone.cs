using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private List<PlayedCard> moves = new List<PlayedCard>();
    [SerializeField] private CardPlayZone physicalZone;
    [SerializeField] private CardPlayZone playedDiscardZone;
    [SerializeField] private GameEvent onFinished;
    [SerializeField] private GameEvent onCardResolved;
    public PlayedCard LastPlayed { get; set; }

    public void Add(PlayedCard played)
    {
        moves.Add(played);
        physicalZone.PutOnBottom(played.Card);
        Debug.Log($"{played.Member.Name} Played {played.Card.name}");
    }

    public void Resolve(MonoBehaviour host)
    {
        host.StartCoroutine(ResolveAll());
    }

    private IEnumerator ResolveAll()
    {
        foreach (var move in moves.ToList())
        {
            yield return ResolveOneCard(move);
            yield return new WaitForSeconds(1.1f);
        }
        onFinished.Publish();
    }
    
    private IEnumerator ResolveOneCard(PlayedCard played)
    {
        var card = physicalZone.DrawOneCard();
        played.Perform();
        LastPlayed = played;
        playedDiscardZone.PutOnBottom(card);
        onCardResolved.Publish();
        yield break;
    }
}
