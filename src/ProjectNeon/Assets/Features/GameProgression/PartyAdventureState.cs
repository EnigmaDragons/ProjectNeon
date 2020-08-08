using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/PartyAdventureState")]
public sealed class PartyAdventureState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private int credits;
    [SerializeField] private int[] nonBattleHp;
    [SerializeField] private RuntimeDeck[] decks;

    public Hero[] Heroes => party.Heroes;
    public int Credits => credits;
    public int[] Hp => nonBattleHp.ToArray();
    public RuntimeDeck[] Decks => decks.ToArray();
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public PartyAdventureState Initialized(Hero one, Hero two, Hero three)
    {
        party.Initialized(one, two, three);
        decks = Heroes.Select(h => CreateDeck(h.Deck)).ToArray();
        nonBattleHp = Heroes.Select(h => h.Stats.MaxHP()).ToArray();
        return this;
    }
    
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

    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);
    
    public void UpdateDecks(List<CardType> one, List<CardType> two, List<CardType> three) 
        => decks = new[] { CreateDeck(one), CreateDeck(two), CreateDeck(three) };


    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };
}
