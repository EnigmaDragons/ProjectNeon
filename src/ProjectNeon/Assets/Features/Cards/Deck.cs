using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Deck")]
public class Deck : ScriptableObject
{
    public List<CardType> Cards;

    public List<CardTypeData> CardTypes => Cards.Cast<CardTypeData>().ToList();
}
