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
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CardPlayZones playerCardPlayZones;
    [SerializeField] private IntReference startingCards;
    
    [Header("Enemies")]
    [SerializeField] private EnemyArea enemyArea;
    [SerializeField] private EncounterBuilder encounterBuilder;
    
    [Header("Technical")]
    [SerializeField] private CardResolutionZone resolutionZone;
    
    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;
    private bool _useCustomEncounter = false;

    public void InitBattleField(GameObject battlefield) => state.SetNextBattleground(battlefield);
    public void InitParty(BaseHero h1, BaseHero h2, BaseHero h3) => party.Initialized(h1, h2, h3);
    public void InitPartyDecks(List<CardType> d1, List<CardType> d2, List<CardType> d3) => party.UpdateDecks(d1, d2, d3);
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
        resolutionZone.Init();
    }    
    
    private void SetupEnemyEncounter()
    {
        BattleLog.Write("Setting Up Enemies");
        if (state.HasCustomEnemyEncounter)
        {
            BattleLog.Write("Setting Up Custom Encounter");
            state.SetupEnemyEncounter();
        }

        if (enemyArea.Enemies.Count == 0)
        {
            BattleLog.Write("Setting Up Fallback Random Encounter");
            enemyArea = enemyArea.Initialized(encounterBuilder.Generate(3));
        }

        foreach (var enemy in enemyArea.Enemies)
        {
            enemy.AI.InitForBattle();
            if (!enemy.IsReadyForPlay)
                throw new Exception($"{enemy.Name}'s is not ready for play.");
            if (enemy.Deck.Cards.All(c => c.Cost.Amount > 0))
                throw new Exception($"{enemy.Name}'s Deck does not contain a 0-Cost Card.");
        }
    }
    
    private void SetupPlayerCards()
    {
        if (!party.IsInitialized)
            throw new Exception("Cannot Setup Player Cards, Party Is Not Initialized");

        playerCardPlayZones.ClearAll();
        var cards = new List<Card>();
        for (var i = 0; i < party.BaseHeroes.Length; i++)
        {
            var hero = party.BaseHeroes[i];
            if (hero == null || hero.Name.Equals(""))
                 continue;
            
            cards.AddRange(party.Decks[i].Cards.Select(c => c.CreateInstance(state.GetNextCardId(), state.GetMemberByHero(hero))));
        }

        Deck.InitShuffled(cards);
        
        for (var c = 0; c < startingCards.Value; c++)
            Hand.PutOnBottom(Deck.DrawOneCard());
    }
}
