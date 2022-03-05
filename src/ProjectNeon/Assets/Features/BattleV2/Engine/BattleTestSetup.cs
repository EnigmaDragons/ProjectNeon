using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTestSetup : MonoBehaviour
{
    [SerializeField] private BattleEngine engine;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattleState state;

    [SerializeField] private bool setupOnAwake = false;
    [SerializeField] private bool setupCardTest = false;
    
    [Header("Party")] 
    [SerializeField] private BaseHero hero1;
    [SerializeField] private BaseHero hero2;
    [SerializeField] private BaseHero hero3;
    [SerializeField] private Deck hero1Deck;
    [SerializeField] private Deck hero2Deck;
    [SerializeField] private Deck hero3Deck;
    [SerializeField] private List<StaticEquipment> hero1Equipment = new List<StaticEquipment>();
    [SerializeField] private List<StaticEquipment> hero2Equipment = new List<StaticEquipment>();
    [SerializeField] private List<StaticEquipment> hero3Equipment = new List<StaticEquipment>();
    [SerializeField] private BaseHero noHero;
    [SerializeField] private BaseHero traineeHero;

    [Header("BattleField")] 
    [SerializeField] private GameObject battlefield;

    [Header("Enemies")] 
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private int stage;

    [Header("Card Test")] 
    [SerializeField] private CardType[] cards;
    [SerializeField] private Library library;

    private void Awake()
    {
        if (!setupOnAwake && state.Party.IsInitialized) 
            return;
        
        if (setupCardTest)
            SetupCardTest();
        else
            UseEverything();
    }
    
    public void UseCustomParty()
    {
        setup.InitParty(hero1 != null && hero1 != noHero ? hero1 : traineeHero, hero2, hero3);
        if (hero1Deck != null)
            setup.InitPartyDecks(hero1Deck.CardTypes, hero2Deck.CardTypes, hero3Deck.CardTypes);
        setup.InitPartyEquipment(hero1Equipment, hero2Equipment, hero3Equipment);
    }

    public void UseCustomBattlefield() => setup.InitBattleField(battlefield);
    public void UseFixedEncounter() => state.SetNextEncounter(enemies.Select(x => x.ForStage(stage)));
    public void UseCustomEncounterSet() => setup.InitEncounterBuilder(encounterBuilder);
    public void SetupBattle() => engine.Setup();

    public void UseEverythingAndStartBattle()
    {
        UseEverything();
        SetupBattle();
    }

    private void UseEverything()
    {
        if (hero1 != null)
            UseCustomParty();
        if (battlefield != null)
            UseCustomBattlefield();
        if (enemies != null && enemies.Any())
            UseFixedEncounter();
        else if (encounterBuilder != null)
            UseCustomBattlefield();
    }

    public void SetupCardTestAndStartBattle()
    {
        SetupCardTest();
        SetupBattle();
    }
    
    public void SetupCardTest()
    {
        var h1Deck = new List<CardTypeData>();
        var h2Deck = new List<CardTypeData>();
        var h3Deck = new List<CardTypeData>();
        
        var cardArchs = cards.First().Archetypes;
        var hero = cardArchs.All(hero1.Archetypes.Contains) 
            ? hero1 
            : library.UnlockedHeroes.First(h => cardArchs.All(h.Archetypes.Contains));

        var h1Cards = cards.Where(c => c.Archetypes.All(hero.Archetypes.Contains)).ToArray();
        Enumerable.Range(0, 12).ForEach(i => h1Deck.Add(h1Cards[i % h1Cards.Length]));
        
        if (hero2 != null)
        {
            var h2Cards = cards.Except(h1Deck).Where(c => c.Archetypes.All(hero2.Archetypes.Contains)).ToArray();
            if (h2Cards.Any())
                Enumerable.Range(0, 12).ForEach(i => h2Deck.Add(h2Cards[i % h2Cards.Length]));
        }
        if (hero3 != null)
        {
            var h3Cards = cards.Except(h1Deck).Except(h2Deck).Where(c => c.Archetypes.All(hero3.Archetypes.Contains)).ToArray();
            if (h3Cards.Any())
                Enumerable.Range(0, 12).ForEach(i => h3Deck.Add(h3Cards[i % h3Cards.Length]));
        }

        setup.InitParty(hero, hero2, hero3);
        setup.InitPartyDecks(h1Deck, h2Deck, h3Deck);
        var equipment = hero.Name.Equals(hero1.Name) 
            ? hero1Equipment : hero.Name.Equals(hero2.Name) 
            ? hero2Equipment : hero.Name.Equals(hero3.Name) 
            ? hero3Equipment : new List<StaticEquipment>();
        setup.InitPartyEquipment(equipment, new List<Equipment>(), new List<Equipment>());
        
        if (battlefield != null)
            UseCustomBattlefield();
        if (enemies != null && enemies.Any())
            UseFixedEncounter();
        else if (encounterBuilder != null)
            UseCustomBattlefield();
    }
}
