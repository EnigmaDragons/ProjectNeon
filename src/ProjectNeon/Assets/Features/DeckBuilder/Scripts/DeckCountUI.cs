using TMPro;
using UnityEngine;

public class DeckCountUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private TextMeshProUGUI deckCount;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference deckSize;
    [SerializeField] private string prefix = "";

    private void UpdateCount()
    {
        deckCount.text = $"{prefix} {state.SelectedHeroesDeck.Deck.Count}/{deckSize.Value}".Trim();
    }

    protected override void Execute(DeckBuilderHeroSelected msg) => UpdateCount();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => UpdateCount();
}
