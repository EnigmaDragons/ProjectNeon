using TMPro;
using UnityEngine;

public class DeckCountUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private TextMeshProUGUI deckCount;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference deckSize;

    private void UpdateCount()
    {
        deckCount.text = $"Deck Size {state.SelectedHeroesDeck.Deck.Count}/{deckSize.Value}";
    }

    protected override void Execute(DeckBuilderHeroSelected msg) => UpdateCount();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => UpdateCount();
}
