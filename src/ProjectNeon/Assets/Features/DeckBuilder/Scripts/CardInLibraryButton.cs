using System;
using TMPro;
using UnityEngine;
using System.Linq;

public class CardInLibraryButton : MonoBehaviour
{
    [SerializeField] private CardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private TextMeshProUGUI numCopiesLabel;

    public void InitInfoOnly(CardTypeData card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "";
    }

    public void InitInfoOnly(Card card)
    {
        presenter.Set(card);
        numCopiesLabel.text = "";
    }
    
    public void Init(CardTypeData card, int numTotal, int numAvailable)
    {
        var action = numAvailable > 0 
            && state.SelectedHeroesDeck.Deck.Count(x => x == card) < 4 
            && (state.SelectedHeroesDeck.Deck.GroupBy(x => x.Name).Count() < 12 || state.SelectedHeroesDeck.Deck.Any(x => x.Name == card.Name))
                ? (Action)(() => AddCard(card))
                : () => { };
        presenter.Set(card, action);
        numCopiesLabel.text = $"{numAvailable}/{numTotal}";
    }
    
    public void InitBasic(CardTypeData card)
    {
        presenter.Set(card, () => { });
        numCopiesLabel.text = "Basic";
    }

    public void AddCard(CardTypeData card)
    {
        state.SelectedHeroesDeck.Deck.Add(card);
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
    }
}
