using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero/2 - Starting Deck", order = -4)]
public class Deck : ScriptableObject
{
    public List<CardType> Cards;

    public List<CardTypeData> CardTypes => Cards.Cast<CardTypeData>().ToList();
}
