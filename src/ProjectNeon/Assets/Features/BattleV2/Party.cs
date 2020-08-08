using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;
    [SerializeField] private RuntimeDeck[] decks;
    
    // Adventure State
    [SerializeField] private int credits;
    [SerializeField] private int[] nonBattleHp;

    private List<Hero> _heroes = new List<Hero>();

    // AdventureState
    public int Credits => credits;
    public int[] Hp => nonBattleHp.ToArray();
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public RuntimeDeck[] Decks => decks.ToArray();
    public Hero[] Heroes => _heroes.ToArray();

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        _heroes = new List<Hero>();
        if (heroOne != null && !heroOne.name.Equals("NoHero"))
            _heroes.Add(heroOne);
        if (heroTwo != null && !heroTwo.name.Equals("NoHero"))
            _heroes.Add(heroTwo);
        if (heroThree != null && !heroThree.name.Equals("NoHero"))
            _heroes.Add(heroThree);
        _heroes.ForEach(h => Debug.Log($"Party Hero: {h.name}"));
        decks = _heroes.Select(h => CreateDeck(h.Deck)).ToArray();
        nonBattleHp = _heroes.Select(h => h.Stats.MaxHP()).ToArray();
        return this;
    }

    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);
    
    public void UpdateDecks(List<CardType> one, List<CardType> two, List<CardType> three) 
        => decks = new[] { CreateDeck(one), CreateDeck(two), CreateDeck(three) };

    // Adventure State
    public void UpdateAdventureHp(int[] hps) => nonBattleHp = hps;
    public void UpdateCreditsBy(int amount) => credits += amount;
    public void HealHeroToFull(Hero hero)
    {
        var index = 0;
        for (; index < Heroes.Length; index++)
        {
            if (Heroes[index].Equals(hero))
                nonBattleHp[index] = Heroes[index].Stats.MaxHP();
        }
    }

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };
}
