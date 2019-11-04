using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveDeckButton : MonoBehaviour
{
    [SerializeField] private Party party;
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Button button;

    public void Save()
    {
        party.UpdateDecks(state.HeroesDecks[0].Deck, state.HeroesDecks[1].Deck, state.HeroesDecks[2].Deck);
    }

    private void Update() 
    {
        button.interactable = state.HeroesDecks.All(x => x.Deck.Cards.Count == deckSize);
    }
}
