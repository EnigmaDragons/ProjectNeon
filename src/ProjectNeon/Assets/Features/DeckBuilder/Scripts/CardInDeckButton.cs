﻿using System.Linq;
using UnityEngine;

public class CardInDeckButton : OnMessage<DeckBuilderCurrentDeckChanged>
{
    [SerializeField] private SimpleDeckCardPresenter presenter;
    [SerializeField] private DeckBuilderState state;
    [SerializeField] private HoverCard hoverCard;

    private Canvas _canvas;
    private Maybe<Card> _card = Maybe<Card>.Missing();
    private CardTypeData _cardType;
    private int _count;
    private GameObject _hoverCard;
    
    private void Awake() => _canvas = FindObjectOfType<Canvas>();
    private void OnDestroy() => OnExit();
    protected override void Execute(DeckBuilderCurrentDeckChanged msg) => UpdateInfo();
    
    public void Init(CardTypeData c)
    {
        _card = Maybe<Card>.Missing();
        _cardType = c;
        UpdateInfo();
    }

    public void Init(Card c)
    {
        _card = c;
        _cardType = c.BaseType;
        UpdateInfo();
    }

    public void RemoveCard()
    {
        state.SelectedHeroesDeck.Deck.Remove(state.SelectedHeroesDeck.Deck.First(x => x.Name == _cardType.Name));
        _count--;
        Message.Publish(new DeckBuilderCurrentDeckChanged(state.SelectedHeroesDeck));
        Message.Publish(new CardRemovedFromDeck(transform));
    }

    public void OnHover()
    {
        _hoverCard = Instantiate(hoverCard.gameObject, _canvas.transform);
        if (_card.IsPresent)
            _hoverCard.GetComponent<HoverCard>().Init(_card.Value);
        else
            _hoverCard.GetComponent<HoverCard>().Init(_cardType);
        Message.Publish(new CardHoveredOnDeck(transform));
    }

    public void OnExit()
    {
        if (_hoverCard != null)
            Destroy(_hoverCard);
    }

    private void UpdateInfo()
    {
        _count = state.SelectedHeroesDeck.Deck.Count(x => x.Name == _cardType.Name);
        if (_card != null && _card.IsPresent)
            presenter.Initialized(_count, _card.Value);
        else
            presenter.Initialized(_count, _cardType);
        presenter.BindLeftClickAction(RemoveCard);
    }
}
