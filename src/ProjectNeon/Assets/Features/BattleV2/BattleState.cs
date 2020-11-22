using System;
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

    [Header("Next Encounter")]
    [SerializeField] private GameObject nextBattlegroundPrototype;
    [SerializeField] private Enemy[] nextEnemies;
    
    [Header("ReadOnly")]
    [SerializeField, ReadOnly] private string[] memberNames;
    [SerializeField, ReadOnly] private int rewardCredits;
    [SerializeField, ReadOnly] private CardType[] rewardCards;
    
    private Queue<Effect> _queuedEffects = new Queue<Effect>();
    public Effect[] QueuedEffects => _queuedEffects.ToArray();
    
    private int _numberOfRecyclesRemainingThisTurn = 0;
    
    public bool SelectionStarted = false;
    public int NumberOfRecyclesRemainingThisTurn => _numberOfRecyclesRemainingThisTurn;
    public int RewardCredits => rewardCredits;
    public CardType[] RewardCards => rewardCards; 
    public bool HasCustomEnemyEncounter => nextEnemies != null && nextEnemies.Length > 0;

    public bool NeedsCleanup => needsCleanup;
    public int PowerLevelRewardFactor => adventure.Adventure.RewardCreditsPerPowerLevel;
    public PartyAdventureState Party => party;
    public PartyArea PartyArea => partyArea;
    public EnemyArea EnemyArea => enemies;
    public GameObject Battlefield => nextBattlegroundPrototype;
    public IDictionary<int, Member> Members => _membersById;
    public Member[] MembersWithoutIds => Members.Values.ToArray();
    public Member[] Heroes => Members.Values.Where(x => x.TeamType == TeamType.Party).ToArray();
    public Member[] Enemies => Members.Values.Where(x => x.TeamType == TeamType.Enemies).ToArray();
    public PlayerState PlayerState => _playerState;
    private Dictionary<int, Enemy> _enemiesById = new Dictionary<int, Enemy>();
    private Dictionary<int, Hero> _heroesById = new Dictionary<int, Hero>();
    private Dictionary<int, Member> _membersById = new Dictionary<int, Member>();
    private Dictionary<int, Transform> _uiTransformsById = new Dictionary<int, Transform>();
    private Dictionary<int, Vector3> _centerPointsById = new Dictionary<int, Vector3>();
    private PlayerState _playerState = new PlayerState(0);
    private Dictionary<int, Member> _unconsciousMembers = new Dictionary<int, Member>();

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
        NextCardId.Reset();
        SelectionStarted = false;
    }

    public int GetNextCardId() => NextCardId.Get();
    
    private int EnemyStartingIndex => 4;
    private int _nextEnemyId = 0;
    public BattleState FinishSetup()
    {
        var id = 0;      
        memberNames = new string[EnemyStartingIndex + enemies.Enemies.Count + 3];
        _uiTransformsById = new Dictionary<int, Transform>();
        _centerPointsById = new Dictionary<int, Vector3>();
        
        var heroes = Party.Heroes;
        _heroesById = new Dictionary<int, Hero>();
        for (var i = 0; i < Party.BaseHeroes.Length; i++)
        {
            id++;
            _heroesById[id] = heroes[i];
            _uiTransformsById[id] = partyArea.UiPositions[i];
            _centerPointsById[id] = partyArea.CenterPositions[i];
            memberNames[id] = heroes[i].Character.Name;
        }

        id = EnemyStartingIndex - 1;
        _enemiesById = new Dictionary<int, Enemy>();
        for (var i = 0; i < enemies.Enemies.Count; i++)
        {
            id++;
            _enemiesById[id] = enemies.Enemies[i];
            _uiTransformsById[id] = enemies.EnemyUiPositions[i];
            _centerPointsById[id] = enemies.CenterPoints[i];
            memberNames[id] = enemies.Enemies[i].name;
        }
        _nextEnemyId = id + 1;
        
        _membersById = _heroesById.Select(m => m.Value.AsMember(m.Key))
            .Concat(_enemiesById.Select(e => e.Value.AsMember(e.Key)))
            .ToDictionary(x => x.Id, x => x);
        
        _playerState = new PlayerState(adventure.Adventure.BaseNumberOfCardCycles);
        
        _numberOfRecyclesRemainingThisTurn = _playerState.CurrentStats.CardCycles();
        rewardCredits = 0;
        rewardCards = new CardType[0];
        needsCleanup = true;
        _queuedEffects = new Queue<Effect>();
        _unconsciousMembers = new Dictionary<int, Member>();
        
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
    public void StartTurn() => UpdateState(() => PlayerState.OnTurnStart());

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
            _numberOfRecyclesRemainingThisTurn = _playerState.CurrentStats.CardCycles();
            PlayerState.OnTurnEnd();
        });

    public void UseRecycle() => UpdateState(() => _numberOfRecyclesRemainingThisTurn--);
    public void AddRewardCredits(int amount) => UpdateState(() => rewardCredits += amount);
    public void SetRewardCards(params CardType[] cards) => UpdateState(() => rewardCards = cards);

    public void AddEnemy(Enemy e, GameObject gameObject) 
        => UpdateState(() =>
        {
            var id = _nextEnemyId++;
            EnemyArea.Add(e, gameObject.transform);
            _enemiesById[id] = e;
            _membersById[id] = e.AsMember(id);
            _uiTransformsById[id] = gameObject.transform;
            var centerPoint = gameObject.GetComponentInChildren<CenterPoint>();
            if (centerPoint == null)
            {
                Log.Error($"{e.Name} is missing a CenterPoint");
                _centerPointsById[id] = Vector3.zero;
            }
            else
                _centerPointsById[id] = centerPoint.transform.position;
            memberNames[id] = e.name;
        });

    // Battle Wrapup
    public void Wrapup()
    {
        RecordPartyAdventureHp();
        GrantRewardCredits();
        GrantRewardCards();
        EnemyArea.Clear();
    }
    
    private void RecordPartyAdventureHp() => Party.UpdateAdventureHp(Heroes.Select(h => Math.Min(h.CurrentHp(), h.State.BaseStats.MaxHp())).ToArray());
    private void GrantRewardCredits() => Party.UpdateCreditsBy(rewardCredits + _playerState.BonusCredits);
    private void GrantRewardCards() => Party.Add(rewardCards);
    
    // Queries
    public bool PlayerWins() =>  Enemies.All(m => m.State.IsUnconscious);
    public bool PlayerLoses() => Heroes.All(m => m.State.IsUnconscious);
    public bool BattleIsOver() => PlayerWins() || PlayerLoses();

    public bool IsHero(int memberId) => _heroesById.ContainsKey(memberId);
    public bool IsEnemy(int memberId) => _enemiesById.ContainsKey(memberId);
    public HeroCharacter GetHeroById(int memberId) => _heroesById[memberId].Character;
    public Enemy GetEnemyById(int memberId) => _enemiesById[memberId];
    public Transform GetTransform(int memberId) => _uiTransformsById[memberId];
    public Vector3 GetCenterPoint(int memberId) => _centerPointsById[memberId];

    public Vector3 GetCenterPoint(Target target)
    {
        if (target.Members.Length == 0)
            return Vector3.zero;
        var bounds = new Bounds(GetCenterPoint(target.Members[0].Id), Vector3.zero);
        for (var i = 1; i < target.Members.Length; i++)
            bounds.Encapsulate(GetCenterPoint(target.Members[i].Id)); 
        return bounds.center;
    }
    public Member GetMemberByHero(HeroCharacter hero) => _membersById[_heroesById.First(x => x.Value.Character == hero).Key];
    public Member GetMemberByEnemyIndex(int enemyIndex) => _membersById.VerboseGetValue(enemyIndex + EnemyStartingIndex, nameof(_membersById));
    public int GetEnemyIndexByMemberId(int memberId) => memberId - EnemyStartingIndex;
    public BattleStateSnapshot GetSnapshot() => new BattleStateSnapshot(_membersById.ToDictionary(m => m.Key, m => m.Value.GetSnapshot()));

    private void UpdateState(Action update)
    {
        var before = GetSnapshot();
        update();
        Message.Publish(new BattleStateChanged(before, this));
    }
}
