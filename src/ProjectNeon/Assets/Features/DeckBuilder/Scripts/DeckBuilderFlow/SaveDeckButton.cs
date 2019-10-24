using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveDeckButton : MonoBehaviour
{
    [SerializeField] private IntReference deckSize;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private Button button;
    [SerializeField] private DeckBuilderNavigation navigation;
    [SerializeField] private GameEvent decksChanged;
    [SerializeField] private DeckStorage storage;

    public void Save()
    {
        if (state.Operation == DeckBuilderOperation.Add)
            storage.AddDeck(state.TemporaryDeck);
        else if (state.Operation == DeckBuilderOperation.Edit)
            state.SelectedDeck.Import(state.TemporaryDeck);
        decksChanged.Publish();
        navigation.NavigateToDeckSelection();
    }

    private void Update() 
    {
        button.interactable = state.TemporaryDeck.Cards.Count == deckSize && !string.IsNullOrWhiteSpace(state.TemporaryDeck.Name);
    }
}
