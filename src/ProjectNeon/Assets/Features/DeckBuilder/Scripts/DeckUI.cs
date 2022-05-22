﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DeckUI : OnMessage<DeckBuilderHeroSelected, DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private CardInDeckButton cardInDeckButtonTemplate;
    [SerializeField] private GameObject emptyCard;
    [SerializeField] private Button clearDeckButton;

    private List<CardInDeckButton> _cardButtons;

    private void Awake()
    {
        if (clearDeckButton != null)
            clearDeckButton.onClick.AddListener(ClearDeck);
    }

    protected override void Execute(DeckBuilderHeroSelected msg) => GenerateDeck();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => OnDeckChanged();

    private void OnDeckChanged() => GenerateDeck();

    public void Init(HeroCharacter character)
    {
        var deck = new RuntimeDeck {Cards = character.Deck.CardTypes};
        state.SelectedHeroesDeck = new HeroesDeck { Hero = new Hero(character, deck), Deck = deck.Cards };
        GenerateDeck();
    }

    public void GenerateDeck()
    {
        _cardButtons = new List<CardInDeckButton>();
        if (state.SelectedHeroesDeck == null)
            return;
        
        var hero = state.SelectedHeroesDeck.Hero;
        pageViewer.Init(cardInDeckButtonTemplate.gameObject, emptyCard, state.SelectedHeroesDeck.Deck
            .Select(x => x.ToNonBattleCard(hero))
            .GroupBy(x => x.Name)
            .OrderBy(x => x.First().Rarity)
            .ThenBy(x => x.Key)
            .Select(x => InitCardInDeckButton(x.First()))
            .ToList(), x => {},
            false);
    }

    private Action<GameObject> InitCardInDeckButton(CardTypeData card)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardInDeckButton = gameObj.GetComponent<CardInDeckButton>();
            cardInDeckButton.Init(card);
            _cardButtons.Add(cardInDeckButton);
        };
        return init;
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

    private void ClearDeck()
    {
        state.SelectedHeroesDeck.Deck.Clear();
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
    }
}
