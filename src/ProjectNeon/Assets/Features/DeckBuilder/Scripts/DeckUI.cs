using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private CardInDeckButton cardInDeckButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private GameEvent heroChanged;
    [SerializeField] private GameEvent deckChanged;

    private List<CardInDeckButton> _cardButtons;

    private void OnEnable()
    {
        heroChanged.Subscribe(GenerateDeck, this);
        deckChanged.Subscribe(OnDeckChanged, this);
    }

    private void OnDisable()
    {
        heroChanged.Unsubscribe(this);
        deckChanged.Unsubscribe(this);
    }

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
            .ToList(), x => {});
    }

    private Action<GameObject> InitCardInDeckButton(Card card)
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
