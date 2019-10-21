using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : ScriptableObject
{
    [SerializeField] private List<Card> cards;

    public List<Card> Cards => cards.ToList();
}
