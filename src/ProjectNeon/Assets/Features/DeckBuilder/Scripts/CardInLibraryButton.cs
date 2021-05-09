using System;
using TMPro;
using UnityEngine;
using System.Linq;

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
        var action = numAvailable > 0 && state.SelectedHeroesDeck.Deck.Count(x => x == card) < 4
            ? (Action)(() => AddCard(card))
            : () => { };
        presenter.Set(card, action);
        numCopiesLabel.text = $"{numAvailable}/{numTotal}";
    }
    
    public void InitBasic(CardType card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "Basic";
    }

    public void AddCard(CardType card)
    {
        state.SelectedHeroesDeck.Deck.Add(card);
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
    }
}
