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

    public void Show(IEnumerable<CardType> deck, Maybe<Member> owner)
    {
        _items = new List<CardNamePresenter>();
        pageViewer.Init(cardTemplate.gameObject, emptyCard, deck
                .OrderBy(x => x.Cost.BaseAmount)
                .ThenBy(x => x.Name)
                .Select(x => InitCardInDeckButton(x, owner))
                .ToList(), x => {},
            false);
    }
    
    private Action<GameObject> InitCardInDeckButton(CardType card, Maybe<Member> owner)
    {
        Action<GameObject> init = gameObj =>
        {
            var cardPresenter = gameObj.GetComponent<CardNamePresenter>();
            cardPresenter.Init(card, owner);
            _items.Add(cardPresenter);
        };
        return init;
    }
}