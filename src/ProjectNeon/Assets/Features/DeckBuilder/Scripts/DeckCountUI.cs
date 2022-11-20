using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCountUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI deckCount;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private IntReference deckSize;
    [SerializeField] private string prefix = "";
    [SerializeField] private Image statusTintTarget;
    [SerializeField] private Color invalidStatusColor;
    
    private Color _originalColor;

    protected override void AfterEnable()
    {
        if (statusTintTarget == null)
            return;

        _originalColor = statusTintTarget.color;
    }

    private void Render()
    {
        deckCount.text = $"{prefix} {state.SelectedHeroesDeck.Deck.Count}/{deckSize.Value}".Trim();
        if (statusTintTarget != null)
            statusTintTarget.color = state.SelectedHeroDeckIsValid ? _originalColor : invalidStatusColor;
    }

    protected override void Execute(DeckBuilderHeroSelected msg) => Render();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => Render();
}
