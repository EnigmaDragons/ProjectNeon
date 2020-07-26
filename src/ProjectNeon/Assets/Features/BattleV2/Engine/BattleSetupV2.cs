using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleSetupV2 : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleWorldVisuals visuals;
    [SerializeField] private BattleUiVisuals ui;

    [Header("Party")]
    [SerializeField] private Party party;
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private IntReference startingCards;
    
    [Header("Enemies")]
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    
    [Header("Technical")]
    [SerializeField] private CardPlayZone resolutionZone;
    
    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;
    private bool _useCustomEncounter = false;

    public void InitBattleField(GameObject battlefield) => state.SetNextBattleground(battlefield);
    public void InitParty(Hero h1, Hero h2, Hero h3) => party.Initialized(h1, h2, h3);
    public void InitPartyDecks(Deck d1, Deck d2, Deck d3) => party.UpdateDecks(d1, d2, d3);
    public void InitEncounterBuilder(EncounterBuilder e) => encounterBuilder = e;
    public void InitEncounter(IEnumerable<Enemy> enemies)
    {
        _useCustomEncounter = true;
        enemyArea.Initialized(enemies);
    }

    public IEnumerator Execute()
    {
        ClearResolutionZone();
        SetupEnemyEncounter();
        yield return visuals.Setup(); // Could Animate
        
        state.Init();
        visuals.AfterBattleStateInitialized();
        
        SetupPlayerCards(); // Could Animate
        ui.Setup();
    }

    private void ClearResolutionZone()
    {
        resolutionZone.Clear();
    }    
    
    private void SetupEnemyEncounter()
    {
        if (enemyArea.Enemies.Length == 0)
            enemyArea = enemyArea.Initialized(encounterBuilder.Generate());
    }
    
    private void SetupPlayerCards()
    {
        if (!party.IsInitialized)
            throw new Exception("Cannot Setup Player Cards, Party Is Not Initialized");

        playerCardPlayZones.ClearAll();
        Deck.InitShuffled(party.Decks.SelectMany(x => x.Cards));
        
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }
}
