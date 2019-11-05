using TMPro;
using UnityEngine;

public class DeckCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deckCount;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent heroSelected;
    [SerializeField] private GameEvent deckChanged;
    [SerializeField] private IntReference deckSize;

    private void OnEnable()
    {
        heroSelected.Subscribe(UpdateCount, this);
        deckChanged.Subscribe(UpdateCount, this);
    }

    private void OnDisable()
    {
        heroSelected.Unsubscribe(this);
        deckChanged.Unsubscribe(this);
    }

    private void UpdateCount()
    {
        deckCount.text = $"Deck Size {state.SelectedHeroesDeck.Deck.Count}/{deckSize.Value}";
    }
}
