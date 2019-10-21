using System.Linq;
using UnityEngine;

public sealed class Party : ScriptableObject
{
    [SerializeField] private Hero heroOne;
    [SerializeField] private Hero heroTwo;
    [SerializeField] private Hero heroThree;
    [SerializeField] private Deck[] decks;

    public Deck[] Decks => decks.ToArray();
    public Hero[] Heroes => new []{ heroOne, heroTwo, heroThree };

    public Party Initialized(Hero one, Hero two, Hero three)
    {
        heroOne = one;
        heroTwo = two;
        heroThree = three;
        decks = new Deck[] { one.Deck, two.Deck, three.Deck };
        return this;
    }
}
