using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/PartyAdventureState")]
public sealed class PartyAdventureState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private int credits;
    [SerializeField] private PartyCardCollection cards;
    
    public int Credits => credits;
    
    public BaseHero[] Heroes => _heroes.Select(h => h.BaseHero).ToArray();
    public int[] Hp =>  _heroes.Select(h => h.CurrentHp).ToArray();
    public RuntimeDeck[] Decks => _heroes.Select(h => h.Deck).ToArray();
    public PartyCardCollection Cards => cards;

    private Hero[] _heroes = new Hero[0];
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public PartyAdventureState Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        party.Initialized(one, two, three);
        _heroes = party.Heroes.Select(h => new Hero(h, CreateDeck(h.Deck))).ToArray();
        credits = 0;
        cards.Initialized(Decks.SelectMany(d => d.Cards));
        return this;
    }
    
    public void UpdateAdventureHp(int[] hps) => UpdateState(() => hps.ForEachIndex((hp, i) => _heroes[i].SetHp(hp)));
    public void UpdateCreditsBy(int amount) => UpdateState(() => credits += amount);

    public int CurrentHpOf(BaseHero hero) => Hp[IndexOf(hero)];
    public void HealHeroToFull(BaseHero hero)
        => UpdateState(() =>
        {
            var index = IndexOf(hero);
            _heroes[index].HealToFull();
        });

    private int IndexOf(BaseHero hero)
    {
        var index = 0;
        for (; index < _heroes.Length; index++)
        {
            if (_heroes[index].BaseHero.Equals(hero))
                return index;
        }
        throw new KeyNotFoundException($"Hero {hero.name} not found in Party");
    }
    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);

    public void UpdateDecks(List<CardType> one, List<CardType> two, List<CardType> three)
    {
        _heroes[0].SetDeck(CreateDeck(one));
        _heroes[1].SetDeck(CreateDeck(two));
        _heroes[2].SetDeck(CreateDeck(three));
    }

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new PartyAdventureStateChanged(this));
    }
}
