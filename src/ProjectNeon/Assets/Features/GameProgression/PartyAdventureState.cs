using System;
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

    public int Credits => credits;
    
    // These probably need to become a structure
    public Hero[] Heroes => party.Heroes;
    public int[] Hp => nonBattleHp.ToArray();
    public RuntimeDeck[] Decks => decks.ToArray();
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public PartyAdventureState Initialized(Hero one, Hero two, Hero three)
    {
        party.Initialized(one, two, three);
        decks = Heroes.Select(h => CreateDeck(h.Deck)).ToArray();
        nonBattleHp = Heroes.Select(h => h.Stats.MaxHp()).ToArray();
        credits = 0;
        return this;
    }
    
    public void UpdateAdventureHp(int[] hps) => UpdateState(() => nonBattleHp = hps);
    public void UpdateCreditsBy(int amount) => UpdateState(() => credits += amount);

    public int CurrentHpOf(Hero hero) => Hp[IndexOf(hero)];
    public void HealHeroToFull(Hero hero)
        => UpdateState(() =>
        {
            var index = IndexOf(hero);
            nonBattleHp[index] = Heroes[index].Stats.MaxHp();
        });

    private int IndexOf(Hero hero)
    {
        var index = 0;
        for (; index < Heroes.Length; index++)
        {
            if (Heroes[index].Equals(hero))
                return index;
        }
        throw new KeyNotFoundException($"Hero {hero.name} not found in Party");
    }
    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);
    
    public void UpdateDecks(List<CardType> one, List<CardType> two, List<CardType> three) 
        => decks = new[] { CreateDeck(one), CreateDeck(two), CreateDeck(three) };

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new PartyAdventureStateChanged(this));
    }
}
