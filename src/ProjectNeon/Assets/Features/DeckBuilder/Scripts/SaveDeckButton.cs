using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveDeckButton : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Button button;

    public void Save()
    {
        party.UpdateDecks(state.HeroesDecks.Select(x => x.Deck).ToArray());
    }

    private void Update() 
    {
        button.interactable = state.HeroesDecks.All(x => x.Deck.Count == deckSize);
    }
}
