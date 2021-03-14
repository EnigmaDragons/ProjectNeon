using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/PartyAdventureState")]
public sealed class PartyAdventureState : ScriptableObject
{
    [SerializeField] private Party party;
    [SerializeField] private int credits;
    [SerializeField] private int numShopRestocks;
    [SerializeField] private PartyCardCollection cards;
    [SerializeField] private PartyEquipmentCollection equipment;
    [SerializeField] private Hero[] heroes = new Hero[0];

    public int NumShopRestocks => numShopRestocks;
    public int Credits => credits;
    
    public HeroCharacter[] BaseHeroes => heroes.Select(h => h.Character).ToArray();
    public Hero[] Heroes => heroes;
    public int[] Hp =>  heroes.Select(h => h.CurrentHp).ToArray();
    public RuntimeDeck[] Decks => heroes.Select(h => h.Deck).ToArray();
    public PartyCardCollection Cards => cards;
    public PartyEquipmentCollection Equipment => equipment;
    
    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    public PartyAdventureState Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        heroes = party.Initialized(one, two, three).Heroes.Select(h => new Hero(h, CreateDeck(h.Deck))).ToArray();
        credits = heroes.Sum(h => h.Character.StartingCredits);
        numShopRestocks = 2;
        
        heroes.ForEach(h =>
        {
            if(!h.Character.DeckIsValid())
                Log.Error($"{h.Name} doesn't have a legal deck");
        });
        
        var allStartingCards = Decks.SelectMany(d => d.Cards)
            .Concat(party.Heroes.SelectMany(h => h.AdditionalStartingCards))
            .ToList();
        cards.Initialized(allStartingCards);
        allStartingCards.Distinct().ForEach(c => cards.EnsureHasAtLeast(c, 4));
        
        equipment = new PartyEquipmentCollection();
        return this;
    }

    public void AwardXp(int xp) => UpdateState(() => heroes.ForEach(h => h.AddXp(xp)));
    public void UpdateAdventureHp(int[] hps) => UpdateState(() => hps.ForEachIndex((hp, i) => heroes[i].SetHp(hp)));
    public void UpdateCreditsBy(int amount) => UpdateState(() => credits += amount);
    public void UpdateNumShopRestocksBy(int amount) => UpdateState(() => numShopRestocks += amount);
    public void ApplyLevelUpPoint(Hero hero, StatAddends stats) => UpdateState(() => hero.ApplyLevelUpPoint(stats));

    public int CurrentHpOf(HeroCharacter hero) => Hp[IndexOf(hero)];
    public void SetHeroHp(Hero h, int hp) => UpdateState(() => h.SetHp(hp));
    public void HealHeroToFull(HeroCharacter hero)
        => UpdateState(() =>
        {
            var index = IndexOf(hero);
            heroes[index].HealToFull();
        });

    private int IndexOf(HeroCharacter hero)
    {
        var index = 0;
        for (; index < heroes.Length; index++)
        {
            if (heroes[index].Character.Equals(hero))
                return index;
        }
        throw new KeyNotFoundException($"Hero {hero.Name} not found in Party");
    }
    public void UpdateDecks(Deck one, Deck two, Deck three) 
        => UpdateDecks(one.Cards, two.Cards, three.Cards);

    public void UpdateDecks(params List<CardType>[] decks) =>
        UpdateState(() =>
        {
            heroes[0].SetDeck(CreateDeck(decks[0]));
            if (heroes.Length > 1)
                heroes[1]?.SetDeck(CreateDeck(decks[1]));
            if (heroes.Length > 2)
                heroes[2]?.SetDeck(CreateDeck(decks[2]));
        });

    public void Add(params CardType[] c) => UpdateState(() => Cards.Add(c));
    public void Add(params Equipment[] e) => UpdateState(() => equipment.Add(e));
    public void EquipTo(Equipment e, Hero h) => UpdateState(() =>
    {
        h.Equip(e);
        equipment.MarkEquipped(e);
        DevLog.Info($"Equipment - Equipped {e.Name} to {h.Name}. Available: {equipment.Available.Count}. Equipped: {equipment.Equipped.Count}");
    });

    public void UnequipFrom(Equipment e, Hero h) => UpdateState(() =>
    {
        h.Unequip(e);
        equipment.MarkUnequipped(e);
        DevLog.Info($"Equipment - Unequipped {e.Name} from {h.Name}. Available: {equipment.Available.Count}. Equipped: {equipment.Equipped.Count}");
    });

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.Cards);
    private RuntimeDeck CreateDeck(List<CardType> cards) => new RuntimeDeck { Cards = cards };

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new PartyAdventureStateChanged(this));
    }

    public static PartyAdventureState InMemory() => (PartyAdventureState) FormatterServices.GetUninitializedObject(typeof(PartyAdventureState));
}
