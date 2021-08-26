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
    [SerializeField] private ShopCardPool allCards;

    private List<CorpCostModifier> corpCostModifiers = new List<CorpCostModifier>();
    private Queue<Blessing> _blessings = new Queue<Blessing>(); 

    public int NumShopRestocks => numShopRestocks;
    public int Credits => credits;
    public int TotalMissingHp => Heroes.Sum(h => h.Health.MissingHp);
    public int TotalNumInjuries => Heroes.Sum(h => h.Health.InjuryNames.Count());

    public HeroCharacter[] BaseHeroes => heroes.Select(h => h.Character).ToArray();
    public Hero[] Heroes => heroes;
    public int[] Hp =>  heroes.Select(h => h.CurrentHp).ToArray();
    public RuntimeDeck[] Decks => heroes.Select(h => h.Deck).ToArray();
    public Blessing[] Blessings => _blessings?.ToArray() ?? (_blessings = new Queue<Blessing>()).ToArray();
    public PartyCardCollection Cards => cards;
    public PartyEquipmentCollection Equipment => equipment;
    private Dictionary<string, List<Hero>> _archKeyHeroes;

    public bool IsInitialized => Decks.Sum(x => x.Cards.Count) >= 12;
    
    public PartyCorpAffinity GetCorpAffinity(Dictionary<string, Corp> allCorps) =>
        PartyCorpAffinityCalculator.ForEquippedEquipment(Heroes.Sum(h => h.Equipment.TotalSlots), allCorps, equipment.Equipped);
    
    public bool HasAnyUnequippedGear()
    {
        var availableGear = Equipment.Available;
        foreach (var gear in availableGear)
            foreach (var h in heroes)
                if (h.Equipment.HasSpareRoomFor(gear))
                {
                    Log.Info($"{h.Name} has room for {gear.Name}");
                    return true;
                }

        return false;
    }

    public PartyAdventureState Initialized(BaseHero one, BaseHero two, BaseHero three)
    {
        party.Initialized(one, two, three);
        var baseHeroes = party.Heroes;
        heroes = baseHeroes.Select(h => new Hero(h, CreateDeck(h.Deck))).ToArray();
        credits = heroes.Sum(h => h.Character.StartingCredits);
        numShopRestocks = 2;
        
        heroes.ForEach(h =>
        {
            if(!h.Character.DeckIsValid())
                Log.Error($"{h.Name} doesn't have a legal deck");
        });


        var allStartingCards = party.Heroes.SelectMany(h => allCards.Get(h.Archetypes, new HashSet<int>(), Rarity.Starter).NumCopies(4)).ToArray();
        cards.Initialized(allStartingCards); 
        
        equipment = new PartyEquipmentCollection();
        InitArchKeyHeroes();
        Log.Info("Party Adventure State Initialized");
        return this;
    }

    public void InitFromSave(BaseHero one, BaseHero two, BaseHero three, int numCredits, CardTypeData[] partyCards, Equipment[] equipments)
    {
        party.Initialized(one, two, three);
        var baseHeroes = party.Heroes;
        heroes = baseHeroes.Select(h => new Hero(h, CreateDeck(h.Deck))).ToArray();
        credits = numCredits;
        cards.Initialized(partyCards);
        equipment = new PartyEquipmentCollection(equipments);
        InitArchKeyHeroes();
    }

    public void AwardXp(int xp) => UpdateState(() => heroes.ForEach(h => h.AddXp(xp)));
    public void UpdateAdventureHp(int[] hps) => UpdateState(() => hps.ForEachIndex((hp, i) => heroes[i].SetHp(hp)));
    public int UpdateCreditsBy(int amount, bool canGoBelowZero = false)
    {
        if (amount == 0)
            return 0;
        
        var creditsBefore = credits;
        UpdateState(() => credits = Mathf.Clamp(credits + amount, canGoBelowZero ? int.MinValue : 0, int.MaxValue));
        if (credits - creditsBefore != 0)
            Message.Publish(new PartyCreditsChanged(creditsBefore, credits));
        return credits - creditsBefore;
    }

    public void UpdateNumShopRestocksBy(int amount) => UpdateState(() => numShopRestocks += amount);

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
        => UpdateDecks(one.CardTypes, two.CardTypes, three.CardTypes);

    public void UpdateDecks(params List<CardTypeData>[] decks) =>
        UpdateState(() =>
        {
            heroes[0].SetDeck(CreateDeck(decks[0]));
            if (heroes.Length > 1)
                heroes[1]?.SetDeck(CreateDeck(decks[1]));
            if (heroes.Length > 2)
                heroes[2]?.SetDeck(CreateDeck(decks[2]));
        });

    public void Add(params CardTypeData[] c) => UpdateState(() => Cards.Add(c));
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

    private RuntimeDeck CreateDeck(Deck deck) => CreateDeck(deck.CardTypes);
    private RuntimeDeck CreateDeck(List<CardTypeData> cards) => new RuntimeDeck { Cards = cards };

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new PartyAdventureStateChanged(this));
    }

    public static PartyAdventureState InMemory() => (PartyAdventureState) FormatterServices.GetUninitializedObject(typeof(PartyAdventureState));
    
    public Hero BestMatchFor(string archetypeKey)
    {
        InitArchKeyHeroes();
        return _archKeyHeroes[archetypeKey].First();
    }

    private void InitArchKeyHeroes()
    {
        _archKeyHeroes = new Dictionary<string, List<Hero>>();
        foreach (var h in Heroes)
        {
            var archKeys = h.Character.ArchetypeKeys();
            foreach (var a in archKeys)
            {
                if (!_archKeyHeroes.ContainsKey(a))
                    _archKeyHeroes[a] = new List<Hero>();
                _archKeyHeroes[a].Add(h);
            };
        }
    }

    public void AddBlessing(Blessing blessing) => _blessings.Enqueue(blessing);
    public void ApplyBlessings(BattleState state)
    {
        if (_blessings.Count > 0)
            Log.Info($"Applying {_blessings.Count} Party Blessings");
        while (_blessings.Count > 0)
            _blessings.Dequeue().Apply(state);
    }

    public HashSet<int> CardsYouCantHaveMoreOf()
    {
        return new HashSet<int>(cards.AllCards
            .Where(card =>
            {
                var heroesThatCanUseThisCard = heroes.Where(hero => card.Key.Archetypes.All(archetype => hero.Character.Archetypes.Contains(archetype)));
                return card.Value >= heroesThatCanUseThisCard.Count() * 4;
            })
            .Select(card => card.Key.Id));
    }

    public CorpCostModifier[] CorpCostModifiers => corpCostModifiers.ToArray();
    public void SetCorpCostModifier(CorpCostModifier[] modifiers) => corpCostModifiers = modifiers?.ToList() ?? new List<CorpCostModifier>();
    public void AddCorpCostModifier(CorpCostModifier modifier) => corpCostModifiers.Add(modifier);

    public float GetCostFactorForEquipment(string corp)
        => Mathf.Clamp(corpCostModifiers
            .Where(x => x.AppliesToEquipmentShop && x.Corp.Equals(corp, StringComparison.OrdinalIgnoreCase))
            .Concat(new[] {new CorpCostModifier {CostPercentageModifier = 1}})
            .Sum(x => x.CostPercentageModifier), 0, 99);
    
    public float GetCostFactorForClinic(string corp)
        => Mathf.Clamp(corpCostModifiers
            .Where(x => x.AppliesToClinic && x.Corp.Equals(corp, StringComparison.OrdinalIgnoreCase))
            .Concat(new[] {new CorpCostModifier {CostPercentageModifier = 1}})
            .Sum(x => x.CostPercentageModifier), 0, 99);
}
