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
    [SerializeField] private PartyEquipmentCollection equipment;
    [SerializeField] private Hero[] heroes = new Hero[0];
    
    public int Credits => credits;
    
    public BaseHero[] BaseHeroes => heroes.Select(h => h.BaseHero).ToArray();
    public Hero[] Heroes => heroes;
    public int[] Hp =>  heroes.Select(h => h.CurrentHp).ToArray();
    public RuntimeDeck[] Decks => heroes.Select(h => h.Deck).ToArray();
    public PartyCardCollection Cards => cards;
    public PartyEquipmentCollection Equipment => equipment;
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public PartyAdventureState Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        heroes = party.Initialized(one, two, three).Heroes.Select(h => new Hero(h, CreateDeck(h.Deck))).ToArray();
        credits = 0;
        cards.Initialized(Decks.SelectMany(d => d.Cards));
        equipment = new PartyEquipmentCollection();
        return this;
    }
    
    public void UpdateAdventureHp(int[] hps) => UpdateState(() => hps.ForEachIndex((hp, i) => heroes[i].SetHp(hp)));
    public void UpdateCreditsBy(int amount) => UpdateState(() => credits += amount);

    public int CurrentHpOf(BaseHero hero) => Hp[IndexOf(hero)];
    public void HealHeroToFull(BaseHero hero)
        => UpdateState(() =>
        {
            var index = IndexOf(hero);
            heroes[index].HealToFull();
        });

    private int IndexOf(BaseHero hero)
    {
        var index = 0;
        for (; index < heroes.Length; index++)
        {
            if (heroes[index].BaseHero.Equals(hero))
                return index;
        }
        throw new KeyNotFoundException($"Hero {hero.name} not found in Party");
    }
    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);

    public void UpdateDecks(List<CardType> one, List<CardType> two, List<CardType> three) =>
        UpdateState(() =>
        {
            heroes[0].SetDeck(CreateDeck(one));
            if (heroes.Length > 1)
                heroes[1]?.SetDeck(CreateDeck(two));
            if (heroes.Length > 2)
                heroes[2]?.SetDeck(CreateDeck(three));
        });

    public void Add(Equipment e) => UpdateState(() => equipment.Add(e));
    public void EquipTo(Equipment e, Hero h) => UpdateState(() => h.Equip(e));

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new PartyAdventureStateChanged(this));
    }
}
