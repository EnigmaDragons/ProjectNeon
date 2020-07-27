using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTestSetup : MonoBehaviour
{
    [SerializeField] private BattleEngine engine;
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattleState state;

    [SerializeField] private bool setupOnAwake = false;
    
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

    private void Awake()
    {
        if (setupOnAwake || !state.Party.IsInitialized)
            UseEverything();
    }
    
    public void UseCustomParty()
    {
        setup.InitParty(hero1, hero2, hero3);
        if (hero1Deck != null)
            setup.InitPartyDecks(hero1Deck, hero2Deck, hero3Deck);
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
}
