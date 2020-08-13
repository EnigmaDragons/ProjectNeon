using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleState : ScriptableObject
{
    [SerializeField] private CardPlayZones cardPlayZones;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private PartyArea partyArea;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EnemyArea enemies;
    [SerializeField] private bool needsCleanup;
    [SerializeField] private int nextCardId;
    
    [Header("Next Encounter")]
    [SerializeField] private GameObject nextBattlegroundPrototype;
    [SerializeField] private Enemy[] nextEnemies;
    
    [Header("ReadOnly")]
    [SerializeField, ReadOnly] private string[] memberNames;
    [SerializeField, ReadOnly] private int rewardCredits;

    private Queue<Effect> _queuedEffects = new Queue<Effect>();
    public Effect[] QueuedEffects => _queuedEffects.ToArray();
    
    private int NumRecyclesPerTurn = 2;
    private int _numberOfRecyclesRemainingThisTurn = 0;
    
    public bool SelectionStarted = false;
    public int NumberOfRecyclesRemainingThisTurn => _numberOfRecyclesRemainingThisTurn;
    public int RewardCredits => rewardCredits;
    public bool HasCustomEnemyEncounter => nextEnemies != null && nextEnemies.Length > 0;

    public bool NeedsCleanup => needsCleanup;
    public PartyAdventureState Party => party;
    public PartyArea PartyArea => partyArea;
    public EnemyArea EnemyArea => enemies;
    public GameObject Battlefield => nextBattlegroundPrototype;
    public IReadOnlyDictionary<int, Member> Members => _membersById;
    public Member[] Heroes => Members.Values.Where(x => x.TeamType == TeamType.Party).ToArray();
    public Member[] Enemies => Members.Values.Where(x => x.TeamType == TeamType.Enemies).ToArray();
    private Dictionary<int, Enemy> _enemiesById = new Dictionary<int, Enemy>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();

    // Setup
    public BattleState Initialized(PartyArea partyArea, EnemyArea enemyArea)
    {
        _queuedEffects = new Queue<Effect>();
        this.partyArea = partyArea;
        enemies = enemyArea;
        return FinishSetup();
    }

    public void SetNextBattleground(GameObject prototype) => nextBattlegroundPrototype = prototype;
    public void SetNextEncounter(IEnumerable<Enemy> e) => nextEnemies = e.ToArray();
    public void SetupEnemyEncounter()
    {
        EnemyArea.Initialized(nextEnemies);
        nextEnemies = new Enemy[0];
    }

    public void Init()
    {
        nextCardId = 0;
    }

    public int GetNextCardId() => nextCardId++;
    
    private int EnemyStartingIndex => 4;
    public BattleState FinishSetup()
    {
        var id = 0;      
        memberNames = new string[EnemyStartingIndex + enemies.Enemies.Length + 3];
        _uiTransformsById = new Dictionary<int, Transform>();
        
        var heroes = Party.Heroes;
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < Party.Heroes.Length; i++)
        {
            id++;
            _heroesById[id] = heroes[i];
            _uiTransformsById[id] = partyArea.UiPositions[i];
            memberNames[id] = heroes[i].name;
        }
        
        _enemiesById = new Dictionary<int, Enemy>();
        for (var i = 0; i < enemies.Enemies.Length; i++)
        {
            id++;
            _enemiesById[id] = enemies.Enemies[i];
            _uiTransformsById[id] = enemies.EnemyUiPositions[i];
            memberNames[id] = enemies.Enemies[i].name;
        }
        
        _membersById = _heroesById.Select(h => new Member(h.Key, h.Value.name, h.Value.Class.Name, TeamType.Party, h.Value.Stats, Party.Hp[h.Key - 1]))
            .Concat(_enemiesById.Select(e => e.Value.AsMember(e.Key)))
            .ToDictionary(x => x.Id, x => x);

        _numberOfRecyclesRemainingThisTurn = NumRecyclesPerTurn;
        rewardCredits = 0;
        needsCleanup = true;
        _queuedEffects = new Queue<Effect>();
        
        BattleLog.Write("Finished Battle State Init");
        return this;
    }
    
    public void CleanupIfNeeded()
    {
        if (!NeedsCleanup) return;
        
        EnemyArea.Clear();
        needsCleanup = false;
        BattleLog.Write("Finished Battle State Cleanup");
    }

    // During Battle State Tracking
    public void StartTurn() =>UpdateState(() => { Members.Values.ForEach(m => m.State.OnTurnStart()); });
    
    public void AdvanceTurn() =>
        UpdateState(() =>
        {
            _numberOfRecyclesRemainingThisTurn = NumRecyclesPerTurn;
            Members.Values.ForEach(m => m.State.OnTurnEnd()); 
        });

    public void UseRecycle() => UpdateState(() => _numberOfRecyclesRemainingThisTurn--);
    public void AddRewardCredits(int amount) => UpdateState(() => rewardCredits += amount);

    public void Queue(Effect e) => UpdateState(() => _queuedEffects.Enqueue(e));
    public Effect DequeueEffect()
    {
        var e = _queuedEffects.Dequeue();
        Message.Publish(new BattleStateChanged(this));
        return e;
    }
    
    // Battle Wrapup
    public void Wrapup()
    {
        RecordPartyAdventureHp();
        GrantRewardCredits();
        EnemyArea.Clear();
    }
    
    private void RecordPartyAdventureHp() => Party.UpdateAdventureHp(Heroes.Select(h => Math.Min(h.CurrentHp(), h.MaxHp())).ToArray());
    private void GrantRewardCredits() => Party.UpdateCreditsBy(rewardCredits);
    
    // Queries
    public bool PlayerWins() =>  Enemies.All(m => m.State.IsUnconscious);
    public bool PlayerLoses() => Heroes.All(m => m.State.IsUnconscious);

    public bool IsHero(int memberId) => _heroesById.ContainsKey(memberId);
    public bool IsEnemy(int memberId) => _enemiesById.ContainsKey(memberId);
    public Hero GetHeroById(int memberId) => _heroesById[memberId];
    public Enemy GetEnemyById(int memberId) => _enemiesById[memberId];
    public Transform GetTransform(int memberId) => _uiTransformsById[memberId];
    public Member GetMemberByHero(Hero hero) => _membersById[_heroesById.First(x => x.Value == hero).Key];
    public Member GetMemberByEnemyIndex(int enemyIndex) => _membersById.VerboseGetValue(enemyIndex + EnemyStartingIndex, nameof(_membersById));
    public int GetEnemyIndexByMemberId(int memberId) => memberId - EnemyStartingIndex;

    private void UpdateState(Action update)
    {
        update();
        Message.Publish(new BattleStateChanged(this));
    }
}
