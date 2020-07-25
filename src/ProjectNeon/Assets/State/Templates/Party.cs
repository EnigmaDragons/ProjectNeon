using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;
    [SerializeField] private RuntimeDeck[] decks;

    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public RuntimeDeck[] Decks => decks.ToArray();
    public Hero[] Heroes => new []{ heroOne, heroTwo, heroThree };

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        decks = new[] { CreateDeck(one.Deck), CreateDeck(two.Deck), CreateDeck(three.Deck) };
        return this;
    }

    public void UpdateDecks(List<Card> one, List<Card> two, List<Card> three)
    {
        decks = new[] { CreateDeck(one), CreateDeck(two), CreateDeck(three) };
    }

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<Card> cards) => new RuntimeDeck { Cards = cards };
}
