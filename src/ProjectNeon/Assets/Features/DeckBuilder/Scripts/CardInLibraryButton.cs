using UnityEngine;

public class CardInLibraryButton : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private GameEvent deckChanged;

    public void Init(Card card)
    {
        presenter.Set(card, () => AddCard(card));
    }

    public void AddCard(Card card)
    {
        state.SelectedHeroesDeck.Deck.Cards.Add(card);
        deckChanged.Publish();
    }
}
