using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private CardInDeckButton cardInDeckButtonTemplate;
    [SerializeField] private GameObject emptyCard;

    private List<CardInDeckButton> _cardButtons;

    protected override void Execute(DeckBuilderHeroSelected msg) => GenerateDeck();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => OnDeckChanged();

    private void OnDeckChanged()
    {
        GenerateDeck();
    } 

    private void GenerateDeck()
    {
        _cardButtons = new List<CardInDeckButton>();
        pageViewer.Init(cardInDeckButtonTemplate.gameObject, emptyCard, state.SelectedHeroesDeck.Deck
            .GroupBy(x => x.Name)
            .Select(x => InitCardInDeckButton(x.First()))
            .ToList(), x => {},
            false);
    }

    private Action<GameObject> InitCardInDeckButton(CardType card)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardInDeckButton = gameObj.GetComponent<CardInDeckButton>();
            cardInDeckButton.Init(card);
            _cardButtons.Add(cardInDeckButton);
        };
        return init;
    }
}
