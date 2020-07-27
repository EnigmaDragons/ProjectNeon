using UnityEngine;

public class CardInLibraryButton : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckChanged;

    public void Init(CardType card)
    {
        presenter.Set(card, () => AddCard(card));
    }

    public void AddCard(CardType card)
    {
        state.SelectedHeroesDeck.Deck.Add(card);
        deckChanged.Publish();
    }
}
