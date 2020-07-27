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
        state.CleanupIfNeeded();
        state.Init();
        ClearResolutionZone();
        SetupEnemyEncounter();
        yield return visuals.Setup(); // Could Animate
        
        state.FinishSetup();
        visuals.AfterBattleStateInitialized();
        
        ui.Setup();
        SetupPlayerCards();
        yield return new WaitForSeconds(1);
    }

    private void ClearResolutionZone()
    {
        resolutionZone.Clear();
    }    
    
    private void SetupEnemyEncounter()
    {
        BattleLog.Write("Setting Up Enemies");
        if (state.HasCustomEnemyEncounter)
        {
            BattleLog.Write("Setting Up Custom Encounter");
            state.SetupEnemyEncounter();
        }

        if (enemyArea.Enemies.Length == 0)
        {
            BattleLog.Write("Setting Up Random Encounter");
            enemyArea = enemyArea.Initialized(encounterBuilder.Generate());
        }
            
        foreach (var enemy in enemyArea.Enemies)
            if (enemy.Deck.Cards.All(c => c.Cost.Amount > 0))
                throw new Exception($"{enemy.Name}'s Deck does not contain a 0-Cost Card.");
    }
    
    private void SetupPlayerCards()
    {
        if (!party.IsInitialized)
            throw new Exception("Cannot Setup Player Cards, Party Is Not Initialized");

        playerCardPlayZones.ClearAll();
        var cards = party.Decks
            .SelectMany(x => x.Cards)
            .Select(x => x.CreateInstance(state.GetNextCardId()));
        Deck.InitShuffled(cards);
        
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }
}
