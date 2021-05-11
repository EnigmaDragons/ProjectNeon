using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroSelectionUI : MonoBehaviour
{
    private const float Padding = 10;

    [SerializeField] private PartyAdventureState party;
    [SerializeField] private DeckBuilderState state;

    public void Init()
    {
        state.HeroesDecks = party.Decks.Select((deck, i) => new HeroesDeck { Deck = deck.Cards.ToList(), Hero = party.Heroes[i]}).ToList();
    }
}
