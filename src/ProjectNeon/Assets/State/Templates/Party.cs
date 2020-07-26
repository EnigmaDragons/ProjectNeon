using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;
    [SerializeField] private RuntimeDeck[] decks;

    private List<Hero> _heroes = new List<Hero>();
    
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
        return this;
    }

    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);
    
    public void UpdateDecks(List<Card> one, List<Card> two, List<Card> three) 
        => decks = new[] { CreateDeck(one), CreateDeck(two), CreateDeck(three) };

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<Card> cards) => new RuntimeDeck { Cards = cards };
}
