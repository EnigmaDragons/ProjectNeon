using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameContent/Deck")]
public class Deck : ScriptableObject
{
    public List<CardType> Cards;
}
