using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    [SerializeField]
    private List<PlayedCard> moves = new List<PlayedCard>();

    public void Add(PlayedCard played)
    {
        moves.Add(played);
    }

    public void Resolve()
    {
        moves.ForEach(
            played => played.Perform()
        );
    }
}
