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

    [Header("BattleField")] 
    [SerializeField] private GameObject battlefield;

    [Header("Enemies")] 
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private EncounterBuilder encounterBuilder;

    [Header("Card Test")] 
    [SerializeField] private CardType[] cards;
    [SerializeField] private BaseHero[] allHeroes;
    [SerializeField] private BaseHero noHero;

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
        setup.InitParty(hero1, hero2, hero3);
        if (hero1Deck != null)
            setup.InitPartyDecks(hero1Deck.Cards, hero2Deck.Cards, hero3Deck.Cards);
        setup.InitPartyEquipment(hero1Equipment, hero2Equipment, hero3Equipment);
    }

    public void UseCustomBattlefield() => setup.InitBattleField(battlefield);
    public void UseFixedEncounter() => state.SetNextEncounter(enemies);
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
        var hero = allHeroes.First(h => h.Class.Equals(cards.First().LimitedToClass.Value));
        setup.InitParty(hero, noHero, noHero);
        setup.InitPartyDecks(Enumerable.Range(0, 12).Select(i => cards[i % cards.Length]).ToList(), new List<CardType>(), new List<CardType>());
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
