using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    [SerializeField] DeckBuilderState deckBuilderState;
    [SerializeField] List<DeckVisualizer> deckVisualizer;

    private void Start()
    {
        deckBuilderState.OnCurrentDeckChanged.Subscribe(OnCharacterChanged, this);
    }
    private void OnCharacterChanged()
    {
        deckVisualizer.ForEach(x => x.UpdateDeckListView());
    }
    private void OnDisable()
    {
        deckBuilderState.OnCurrentDeckChanged.Unsubscribe(this);
    }
}
