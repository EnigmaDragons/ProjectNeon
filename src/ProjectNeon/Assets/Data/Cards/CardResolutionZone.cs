using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardResolutionZone : ScriptableObject
{
    private List<PlayedCard> enemyMoves = new List<PlayedCard>();

    public void AddEnemyMove(PlayedCard played)
    {
        enemyMoves.Add(played);
    }
}
