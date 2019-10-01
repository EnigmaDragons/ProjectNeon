using UnityEngine;

public class PartyDecks : ScriptableObject
{
    [SerializeField] private Deck[] decks;

    public Deck[] Decks => decks;
}
