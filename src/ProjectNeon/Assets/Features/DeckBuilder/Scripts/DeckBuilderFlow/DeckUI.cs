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
    [SerializeField] private GameEvent deckChosen;
    [SerializeField] private GameEvent deckChanged;

    private List<CardInDeckButton> _cardButtons;

    private void OnEnable()
    {
        deckChosen.Subscribe(GenerateDeck, this);
        deckChanged.Subscribe(OnDeckChanged, this);
    }

    private void OnDisable()
    {
        deckChosen.Unsubscribe(this);
        deckChanged.Unsubscribe(this);
    }

    private void OnDeckChanged()
    {
        GenerateDeck();
    } 

    private void GenerateDeck()
    {
        _cardButtons = new List<CardInDeckButton>();
        pageViewer.Init(cardInDeckButtonTemplate.gameObject, emptyCard, state.TemporaryDeck.Cards
            .Select(x => x.Name)
            .Distinct()
            .Select(InitCardInDeckButton)
            .ToList(), x => {});
    }

    private Action<GameObject> InitCardInDeckButton(string cardName)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardInDeckButton = gameObj.GetComponent<CardInDeckButton>();
            cardInDeckButton.Init(cardName);
            _cardButtons.Add(cardInDeckButton);
        };
        return init;
    }
}
