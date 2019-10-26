using System.Collections.Generic;
using UnityEngine;

public class AddDeckButton : MonoBehaviour
{
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckBuildingStarted;

    public void AddDeck()
    {
        state.Operation = DeckBuilderOperation.Add;
        state.SelectedDeck = new Deck { Name = "", ClassName = state.SelectedHero.ClassName, Cards = new List<Card>(), IsImmutable = false };
        deckBuildingStarted.Publish();
    }
}
