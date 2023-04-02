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
    [SerializeField] private EncounterBuilderV5 encounterBuilder;
    
    [Header("Technical")]
    [SerializeField] private CardResolutionZone resolutionZone;
    
    private DeterministicRng _battleRng;
    
    private CardPlayZone Hand => playerCardPlayZones.HandZone;
    private CardPlayZone Deck => playerCardPlayZones.DrawZone;

    private void Awake() => Log.Info("BattleSetupV2");
    
    public void InitBattleField(GameObject battlefield) => state.SetNextBattleground(battlefield);
    public void InitParty(BaseHero h1, BaseHero h2, BaseHero h3) => party.Initialized(h1, h2, h3);
    public void InitPartyEquipment(IEnumerable<StaticEquipment> h1, IEnumerable<StaticEquipment> h2, IEnumerable<StaticEquipment> h3)
    {
        h1.Concat(h2).Concat(h3).ForEach(e => party.Add(e));
        h1.ForEach(e => InitEquipmentForHero(party.Heroes[0], e));
        h2.ForEach(e => InitEquipmentForHero(party.Heroes[1], e));
        h3.ForEach(e => InitEquipmentForHero(party.Heroes[2], e));
    }
    private void InitEquipmentForHero(Hero hero, StaticEquipment equip)
    {
        if (equip.Slot == EquipmentSlot.Permanent)
            hero.ApplyPermanent(equip);
        else
            party.EquipTo(equip, hero);
    }
    public void InitPartyDecks(List<CardTypeData> d1, List<CardTypeData> d2, List<CardTypeData> d3) => party.UpdateDecks(d1, d2, d3);
    public void InitEncounterBuilder(EncounterBuilderV5 e) => encounterBuilder = e;
    public void InitEncounter(IEnumerable<EnemyInstance> enemies) => enemyArea.Initialized(enemies);
    public void InitRng(DeterministicRng rng) => _battleRng = rng;

    public IEnumerator ExecuteCharacters()
    {
        state.CleanupIfNeeded();
        state.Init();
        ClearResolutionZone();
        SetupEnemyEncounter();
        yield return visuals.Setup(); // Could Animate

        playerCardPlayZones.ClearAll();
        var enemies = state.FinishSetup();
        visuals.Setup2(enemies);
        visuals.AfterBattleStateInitialized();
        ui.Setup();
        DevLog.Write("Finished Character Setup");
    }
    
    public IEnumerator ExecuteCards()
    {
        Message.Publish(new PlayerDeckShuffled()); // Play sound early for flow
        yield return new WaitForSeconds(0.1f);
        yield return SetupPlayerCards();
        yield return new WaitForSeconds(1.05f);
        DevLog.Write("Finished Card Setup");
    }

    private void ClearResolutionZone()
    {
        resolutionZone.Init();
    }    
    
    private void SetupEnemyEncounter()
    {
        DevLog.Write("Setting Up Enemies");
        if (state.HasCustomEnemyEncounter)
        {
            DevLog.Write("Setting Up Custom Encounter");
            state.SetupEnemyEncounter();
        }

        if (enemyArea.Enemies.Count == 0)
        {
            DevLog.Write("Setting Up Fallback Random Encounter of Difficulty 250");
            enemyArea = enemyArea.Initialized(encounterBuilder.Generate(250, 1, false));
        }

        foreach (var enemy in enemyArea.Enemies)
        {
            if (enemy.AI == null)
                throw new Exception($"{enemy.NameTerm.ToEnglish()} does not have an AI");
            if (!enemy.IsReadyForPlay)
                throw new Exception($"{enemy.NameTerm.ToEnglish()} is not ready for play.");
            if (enemy.Cards.All(c => c.Cost.BaseAmount > 0))
                throw new Exception($"{enemy.NameTerm.ToEnglish()}'s Deck does not contain a 0-Cost Card.");
            enemy.AI.InitForBattle();
        }
    }
    
    private IEnumerator SetupPlayerCards()
    {
        if (!party.IsInitialized)
            throw new Exception("Cannot Setup Player Cards, Party Is Not Initialized");
        
        var cards = new List<Card>();
        for (var i = 0; i < party.BaseHeroes.Length; i++)
        {
            var hero = party.BaseHeroes[i];
            if (hero == null)
                 continue;

            var cardTypes = state.OverrideDeck == null ? party.Decks[i].Cards : state.OverrideDeck.Cast<CardTypeData>();
            
            cards.AddRange(cardTypes.Select(c => c.CreateInstance(state.GetNextCardId(), state.GetMemberByHero(hero), hero.Tint, hero.Bust)));
        }

        DevLog.Write($"Setting Up Player Hand - Should Shuffle {!state.DontShuffleNextBattle} - Rng Seed {state.BattleRngSeed}");
        var rng = _battleRng ?? new DeterministicRng(state.BattleRngSeed);
        if (state.DontShuffleNextBattle)
            Deck.Init(cards);
        else
            Deck.InitShuffled(cards, rng);

        var cardsToDraw = state.OverrideStartingPlayerCards.Select(numCards => numCards, () => state.PlayerState.CardDraws);
        yield return playerCardPlayZones.DrawHandAsync(cardsToDraw);
    }
}
