using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReadOnlyEnemyDeckUI : MonoBehaviour
{
    [SerializeField] private PageViewer pageViewer;
    [SerializeField] private CardNamePresenter cardTemplate;
    [SerializeField] private GameObject emptyCard;

    private List<CardNamePresenter> _items;

    public void Show(Deck deck)
    {
        _items = new List<CardNamePresenter>();
        pageViewer.Init(cardTemplate.gameObject, emptyCard, deck.Cards
                .OrderBy(x => x.Cost.BaseAmount)
                .ThenBy(x => x.Name)
                .Select(InitCardInDeckButton)
                .ToList(), x => {},
            false);
    }
    
    private Action<GameObject> InitCardInDeckButton(CardType card)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardPresenter = gameObj.GetComponent<CardNamePresenter>();
            cardPresenter.Init(card);
            _items.Add(cardPresenter);
        };
        return init;
    }
}