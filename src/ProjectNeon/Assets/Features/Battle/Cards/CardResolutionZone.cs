using System.Collections.Generic;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    [SerializeField] private List<PlayedCard> moves = new List<PlayedCard>();
    [SerializeField] private CardPlayZone physicalZone;
    
    public void Add(PlayedCard played)
    {
        moves.Add(played);
        physicalZone.PutOnBottom(played.Card);
        Debug.Log($"{played.Member.Name} Played {played.Card.name}");
    }

    public void Resolve()
    {
        moves.ForEach(played => played.Perform());
    }
}
