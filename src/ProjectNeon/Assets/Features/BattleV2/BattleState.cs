﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "GameState/BattleState")]
public class BattleState : ScriptableObject
{
    [SerializeField] private CardPlayZones cardPlayZones;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EnemyArea enemies;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private bool needsCleanup;
    [SerializeField] private bool isEliteBattle;
    [SerializeField] private BattleV2Phase phase;
    [SerializeField] private AdventureProgress2 adventureProgress;
    
    [Header("Next Encounter")]
    [SerializeField] private GameObject nextBattlegroundPrototype;
    [SerializeField] private EnemyInstance[] nextEnemies;
    [SerializeField] private bool nextIsEliteBattle;
    
    [Header("ReadOnly")]
    [SerializeField, ReadOnly] private List<string> memberNames;
    [SerializeField, ReadOnly] private int rewardCredits;
    [SerializeField, ReadOnly] private CardTypeData[] rewardCards;
    [SerializeField, ReadOnly] private Equipment[] rewardEquipments;
    [SerializeField, ReadOnly] private int rewardXp = 0;
    [SerializeField, ReadOnly] private int turnNumber;
    [SerializeField, ReadOnly] private PlayerState playerState = new PlayerState();
    
    private Queue<Effect> _queuedEffects = new Queue<Effect>();
    private List<List<PlayedCardSnapshot>> _playedCardHistory = new List<List<PlayedCardSnapshot>>();
    public Effect[] QueuedEffects => _queuedEffects.ToArray();
    
    private int _numPlayerDiscardsUsedThisTurn = 0;

    public int CreditsAtStartOfBattle { get; private set; }
    public bool IsSelectingTargets = false;
    public BattleV2Phase Phase => phase;
    public int TurnNumber => turnNumber;
    public int NumberOfRecyclesRemainingThisTurn => PlayerState.NumberOfRecyclesRemainingThisTurn;
    private int CurrentTurnPartyNonBonusStandardCardPlays => CurrentTurnCardPlays()
        .Count(x => x.Member.TeamType == TeamType.Party && !x.WasTransient && x.Card.Speed == CardSpeed.Standard);
    public int NumberOfCardPlaysRemainingThisTurn => playerState.CurrentStats.CardPlays() - CurrentTurnPartyNonBonusStandardCardPlays - _numPlayerDiscardsUsedThisTurn;
    public bool HasMorePlaysAvailableThisTurn =>
        NumberOfCardPlaysRemainingThisTurn > 0 ||
        NumberOfRecyclesRemainingThisTurn > 0 ||
        cardPlayZones.HandZone.Cards.Any(x => x.IsActive && x.Speed == CardSpeed.Quick);
    public PlayedCardSnapshot[] CurrentTurnCardPlays() => _playedCardHistory.Any()
        ? _playedCardHistory.Last().ToArray()
        : Array.Empty<PlayedCardSnapshot>();
    
    public int RewardCredits => rewardCredits;
    public int RewardXp => rewardXp;
    public CardTypeData[] RewardCards => rewardCards; 
    public bool HasCustomEnemyEncounter => nextEnemies != null && nextEnemies.Length > 0;
    public EnemyInstance[] NextEncounterEnemies => nextEnemies.ToArray();
    public int Stage => adventureProgress.CurrentChapterNumber;
    public bool NeedsCleanup => needsCleanup;
    public bool IsEliteBattle => isEliteBattle;
    public float CreditPerPowerLevelRewardFactor => adventure.Adventure?.RewardCreditsPerPowerLevel ?? 0;
    public float XpPerPowerLevelRewardFactor => adventure.Adventure?.XpPerPowerLevel ?? 0;
    public CardPlayZones PlayerCardZones => cardPlayZones;
    public PartyAdventureState Party => party;
    public PartyArea PartyArea => partyArea;
    public EnemyArea EnemyArea => enemies;
    public GameObject Battlefield => nextBattlegroundPrototype;
    public IDictionary<int, Member> Members => _membersById;
    public Member[] MembersWithoutIds => Members.Values.ToArray();
    public Member[] Heroes => Members.Values.Where(x => x.TeamType == TeamType.Party).ToArray();
    public Member[] EnemyMembers => Members.Values.Where(x => x.TeamType == TeamType.Enemies).ToArray();
    public (Member Member, EnemyInstance Enemy)[] Enemies => EnemyMembers.Select(m => (m, _enemiesById[m.Id])).ToArray();
    public PlayerState PlayerState => playerState;
    private Dictionary<int, EnemyInstance> _enemiesById = new Dictionary<int, EnemyInstance>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();
    private Dictionary<int, Member> _unconsciousMembers = new Dictionary<int, Member>();

    // Setup

    public void SetNextBattleground(GameObject prototype) => nextBattlegroundPrototype = prototype;
    public void SetNextEncounter(IEnumerable<EnemyInstance> e, bool isElite = false)
    {
        nextEnemies = e.ToArray();
        nextIsEliteBattle = isElite;
        DevLog.Write($"Next Encounter has {string.Join(", ", e.Select(x => x.Name))}");
    }

    public void SetupEnemyEncounter()
    {
        EnemyArea.Initialized(nextEnemies);
        isEliteBattle = nextIsEliteBattle;
        nextEnemies = new EnemyInstance[0];
        nextIsEliteBattle = false;
    }

    public void Init()
    {
        SetPhase(BattleV2Phase.NotBegun);
        NextCardId.Reset();
        IsSelectingTargets = false;
        turnNumber = 0;
    }

    public void SetPhase(BattleV2Phase p)
    {
        UpdateState(() => phase = p);
    }

    public int GetNextCardId() => NextCardId.Get();

    private int EnemyStartingIndex => 4;
    private int _nextEnemyId = 0;
    public int GetNextEnemyId() => _nextEnemyId++;
    public List<Tuple<int, Member>> FinishSetup()
    {
        var id = 0;      
        memberNames = new List<string>();
        _uiTransformsById = new Dictionary<int, Transform>();

        var heroes = Party.Heroes;
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < Party.BaseHeroes.Length; i++)
        {
            id++;
            _heroesById[id] = heroes[i];
            _uiTransformsById[id] = partyArea.UiPositions[i];
            _uiTransformsById[id].GetComponentInChildren<ActiveMemberIndicator>()?.Init(id, true);
            SetMemberName(id, heroes[i].Character.Name);
        }

        id = EnemyStartingIndex - 1;
        _enemiesById = new Dictionary<int, EnemyInstance>();
        var result = new List<Tuple<int, Member>>();
        for (var i = 0; i < enemies.Enemies.Count; i++)
        {
            id++;
            _enemiesById[id] = enemies.Enemies[i];
            _uiTransformsById[id] = enemies.EnemyUiPositions[i];
            _uiTransformsById[id].GetComponent<ActiveMemberIndicator>()?.Init(id, false);
            SetMemberName(id, enemies.Enemies[i].Name);
            result.Add(new Tuple<int, Member>(i, _enemiesById[id].AsMember(id)));
        }
        _nextEnemyId = id + 1;

        playerState = new PlayerState(adventure?.Adventure?.BaseNumberOfCardCycles ?? 0);
        _membersById = _heroesById.Select(m => m.Value.AsMember(m.Key))
            .Concat(result.Select(e => e.Item2))
            .ToDictionary(x => x.Id, x => x);
        
        _heroesById.ForEach(h => h.Value.InitEquipmentState(_membersById[h.Key], this));
        _enemiesById.ForEach(e => e.Value.SetupMemberState(_membersById[e.Key], this));

        PlayerState.NumberOfCyclesUsedThisTurn = 0;
        _numPlayerDiscardsUsedThisTurn = 0;
        rewardCredits = 0;
        rewardCards = new CardType[0];
        rewardEquipments = new Equipment[0];
        rewardXp = 0;
        needsCleanup = true;
        _queuedEffects = new Queue<Effect>();
        _unconsciousMembers = new Dictionary<int, Member>();
        _playedCardHistory = new List<List<PlayedCardSnapshot>> { new List<PlayedCardSnapshot>() };
        turnNumber = 1;
        CreditsAtStartOfBattle = party.Credits;
        party.ApplyBlessings(this);
        
        DevLog.Write("Finished Battle State Init");
        return result;
    }

    public void CleanupIfNeeded()
    {
        if (!NeedsCleanup) return;
        
        EnemyArea.Clear();
        needsCleanup = false;
        DevLog.Write("Finished Battle State Cleanup");
    }

    // During Battle State Tracking
    public void StartTurn() => UpdateState(() => PlayerState.OnTurnStart());
    public void RecordPlayedCard(IPlayedCard card) => UpdateState(() => _playedCardHistory.Last().Add(new PlayedCardSnapshot(card)));
    public void RecordCardDiscarded() => UpdateState(() => _numPlayerDiscardsUsedThisTurn++);
    public void RemoveLastRecordedPlayedCard() => _playedCardHistory.Last().RemoveLast();
    public void CleanupExpiredMemberStates() => UpdateState(() => _membersById.ForEach(x => x.Value.State.CleanExpiredStates()));

    public Member[] GetAllNewlyUnconsciousMembers()
    {
        var newlyUnconscious = Members
            .Where(m => !_unconsciousMembers.ContainsKey(m.Key))
            .Select(m => m.Value)
            .Where(m => !m.State.IsConscious)
            .ToArray();
        newlyUnconscious.ForEach(r => _unconsciousMembers[r.Id] = r);
        return newlyUnconscious;
    }

    public Member[] GetAllNewlyRevivedMembers()
    {
        var newlyConscious = Members
            .Where(m => _unconsciousMembers.ContainsKey(m.Key))
            .Select(m => m.Value)
            .Where(m => m.State.IsConscious)
            .ToArray();
        newlyConscious.ForEach(r => _unconsciousMembers.Remove(r.Id));
        return newlyConscious;
    }
    
    public void AdvanceTurn() =>
        UpdateState(() =>
        {
            _playedCardHistory.Add(new List<PlayedCardSnapshot>());
            _numPlayerDiscardsUsedThisTurn = 0;
            PlayerState.OnTurnEnd();
            turnNumber++;
        });

    public void UseRecycle() => UpdateState(() => PlayerState.NumberOfCyclesUsedThisTurn++);
    public void AddRewardCredits(int amount) => UpdateState(() => rewardCredits += amount);
    public void AddRewardXp(int xp) => UpdateState(() => rewardXp += xp); 
    public void SetRewardCards(params CardTypeData[] cards) => UpdateState(() => rewardCards = cards);
    public void SetRewardEquipment(params Equipment[] equipments) => UpdateState(() => rewardEquipments = equipments);

    public void AddEnemy(EnemyInstance e, GameObject gameObject, Member member) 
        => UpdateState(() =>
        {
            EnemyArea.Add(e, gameObject.transform);
            _enemiesById[member.Id] = e;
            _membersById[member.Id] = member;
            _uiTransformsById[member.Id] = gameObject.transform;
            SetMemberName(member.Id, e.Name);
            e.SetupMemberState(member, this);
        });

    public void RemoveEnemy(MemberState member)
        => UpdateState(() =>
        {
            var enemy = _enemiesById[member.MemberId];
            EnemyArea.Remove(enemy);
            _enemiesById.Remove(member.MemberId);
            _membersById.Remove(member.MemberId);
            _uiTransformsById.Remove(member.MemberId);
            
        });

    private void SetMemberName(int id, string name)
    {
        while (memberNames.Count <= id)
            memberNames.Add("");
        memberNames[id] = name;
    }
    
    // Battle Wrapup
    public void Wrapup()
    {
        RecordPartyAdventureHp();
        GrantRewardCredits();
        GrantRewardCards();
        GrantRewardEquipment();
        GrantRewardXp();
        EnemyArea.Clear();
    }
    
    private void RecordPartyAdventureHp() => Party.UpdateAdventureHp(Heroes.Select(h => Math.Min(h.CurrentHp(), h.State.BaseStats.MaxHp())).ToArray());
    private void GrantRewardCredits() => Party.UpdateCreditsBy(rewardCredits + playerState.BonusCredits);
    private void GrantRewardCards() => Party.Add(rewardCards);
    private void GrantRewardEquipment() => Party.Add(rewardEquipments);
    private void GrantRewardXp() => Party.AwardXp(rewardXp);
    
    // Queries
    public bool PlayerWins() =>  EnemyMembers.All(m => m.State.IsUnconscious);
    public bool PlayerLoses() => Heroes.All(m => m.State.IsUnconscious);
    public bool BattleIsOver() => PlayerWins() || PlayerLoses();

    public bool IsHero(int memberId) => _heroesById.ContainsKey(memberId);
    public bool IsEnemy(int memberId) => _enemiesById.ContainsKey(memberId);
    public HeroCharacter GetHeroById(int memberId) => _heroesById[memberId].Character;
    public EnemyInstance GetEnemyById(int memberId) => _enemiesById[memberId];
    public Transform GetTransform(int memberId) => _uiTransformsById[memberId];

    public Transform GetCenterPoint(int memberId)
    {
        var memberTransform = GetTransform(memberId);
        var centerPoint = memberTransform.GetComponentInChildren<CenterPoint>();
        if (centerPoint != null)
            return centerPoint.transform;
        Log.Warn($"Center Point Missing For: {_membersById[memberId].Name}");
        return memberTransform;
    }

    public Vector3 GetCenterPoint(Target target)
    {
        if (target.Members.Length == 0)
            return Vector3.zero;
        var bounds = new Bounds(GetCenterPoint(target.Members[0].Id).position, Vector3.zero);
        for (var i = 1; i < target.Members.Length; i++)
            bounds.Encapsulate(GetCenterPoint(target.Members[i].Id).position); 
        return bounds.center;
    }
    public Member GetMemberByHero(HeroCharacter hero) => _membersById[_heroesById.First(x => x.Value.Character == hero).Key];
    public Member GetMemberByEnemyIndex(int enemyIndex) => _membersById.VerboseGetValue(enemyIndex + EnemyStartingIndex, nameof(_membersById));
    public int GetEnemyIndexByMemberId(int memberId) => memberId - EnemyStartingIndex;
    public BattleStateSnapshot GetSnapshot() => 
        new BattleStateSnapshot(Phase, _playedCardHistory.Select(x => x.ToArray()).ToList(), 
            _membersById.ToDictionary(m => m.Key, m => m.Value.GetSnapshot()));

    private void UpdateState(Action update)
    {
        var before = GetSnapshot();
        update();
        Message.Publish(new BattleStateChanged(before, this));
    }
}
