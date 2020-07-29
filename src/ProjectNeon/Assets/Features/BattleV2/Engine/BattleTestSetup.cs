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
    [SerializeField] private Hero hero1;
    [SerializeField] private Hero hero2;
    [SerializeField] private Hero hero3;
    [SerializeField] private Deck hero1Deck;
    [SerializeField] private Deck hero2Deck;
    [SerializeField] private Deck hero3Deck;

    [Header("BattleField")] 
    [SerializeField] private GameObject battlefield;

    [Header("Enemies")] 
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private EncounterBuilder encounterBuilder;

    [Header("Card Test")] 
    [SerializeField] private CardType card;
    [SerializeField] private Hero[] allHeroes;
    [SerializeField] private Hero noHero;

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
        var hero = allHeroes.First(h => h.Class.Equals(card.LimitedToClass.Value));
        setup.InitParty(hero, noHero, noHero);
        setup.InitPartyDecks(Enumerable.Range(0, 12).Select(i => card).ToList(), new List<CardType>(), new List<CardType>());
        
        if (battlefield != null)
            UseCustomBattlefield();
        if (enemies != null && enemies.Any())
            UseFixedEncounter();
        else if (encounterBuilder != null)
            UseCustomBattlefield();
    }
}
