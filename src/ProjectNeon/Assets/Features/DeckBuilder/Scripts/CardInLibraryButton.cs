using System;
using TMPro;
using UnityEngine;

public class CardInLibraryButton : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI numCopiesLabel;

    public void InitInfoOnly(CardType card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "";
    }
    
    public void Init(CardType card, int numTotal, int numAvailable)
    {
        var action = numAvailable > 0 
            ? (Action)(() => AddCard(card))
            : () => { };
        presenter.Set(card, action);
        numCopiesLabel.text = $"{numAvailable}/{numTotal}";
    }

    public void AddCard(CardType card)
    {
        state.SelectedHeroesDeck.Deck.Add(card);
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
    }
}
