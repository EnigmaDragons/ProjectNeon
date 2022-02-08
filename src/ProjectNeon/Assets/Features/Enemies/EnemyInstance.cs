using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnemyInstance : EnemyType
{
    private readonly int _enemyId;
    private readonly IResourceType _resourceType;
    private readonly MemberMaterialType _materialType;
    private readonly EffectData[] _startOfBattleEffects;
    private readonly int _startingResourceAmount;
    private readonly int _resourceGainPerTurn;
    private readonly int _maxResourceAmount;
    private readonly int _maxHp;
    private readonly int _maxShield;
    private readonly int _startingShield;
    private readonly int _attack;
    private readonly int _magic;
    private readonly int _leadership;
    private readonly float _armor;
    private readonly float _resistance;
    private readonly int _cardsPerTurn;
    private readonly Dictionary<string, int> _counterAdjustments;
    private readonly Corp _corp;

    public int EnemyId => _enemyId;
    public EffectData[] StartOfBattleEffects => _startOfBattleEffects;
    public GameObject Prefab { get; }
    public Vector3 LibraryCameraOffset { get; }
    public TurnAI AI { get; }
    public IEnumerable<CardType> Cards { get; }
    public BattleRole Role { get; }
    public EnemyTier Tier { get; }
    public Corp Corp => _corp;
    public int PowerLevel { get; }
    public int PreferredTurnOrder { get; }
    public string Name { get; }
    public string DeathEffect { get; }
    public bool IsHasty { get; }
    public bool IsUnique { get; }
    public int ResourceGainPerTurn => _resourceGainPerTurn;
    public int StartingResourceAmount => _startingResourceAmount;
    public int MaxResourceAmount => _maxResourceAmount;
    public CharacterAnimations Animations { get; }
    public AiPreferences AIPreferences { get; }
    public IEnumerable<ReactionCardType> ReactionCards { get; }
    public string Description { get; }

    public bool DeckIsValid => Cards.None(x => x == null);
    public bool IsReadyForPlay => Cards != null && Prefab != null && AI != null;

    public EnemyInstance(int enemyId, ResourceType resourceType, EffectData[] startOfBattleEffects, int startingResourceAmount, 
        int resourceGainPerTurn, int maxResourceAmount, int maxHp, int maxShield, int startingShield, 
        int attack, int magic, int leadership, float armor, float resistance, int cardsPerTurn, 
        GameObject prefab, Vector3 libraryCameraOffset, TurnAI ai, IEnumerable<CardType> cards, BattleRole role, EnemyTier tier, int powerLevel, 
        int preferredTurnOrder, string enemyName, string deathEffect, bool isHasty, bool isUnique, Dictionary<string, int> counterAdjustments, Corp corp,
        CharacterAnimations animations, MemberMaterialType materialType, string description, IEnumerable<ReactionCardType> reactionCards, AiPreferences aiPreferences)
    {
        _enemyId = enemyId;
        _resourceType = resourceType;
        _startOfBattleEffects = startOfBattleEffects;
        _startingResourceAmount = startingResourceAmount;
        _resourceGainPerTurn = resourceGainPerTurn;
        _maxResourceAmount = maxResourceAmount;
        _maxHp = maxHp;
        _maxShield = maxShield;
        _startingShield = startingShield;
        _attack = attack;
        _magic = magic;
        _leadership = leadership;
        _armor = armor;
        _resistance = resistance;
        _cardsPerTurn = cardsPerTurn;
        _counterAdjustments = counterAdjustments;
        _materialType = materialType;
        _corp = corp != null ? corp : new InMemoryCorp();
        Prefab = prefab;
        LibraryCameraOffset = libraryCameraOffset;
        AI = ai;
        Cards = cards;
        Role = role;
        Tier = tier;
        PowerLevel = powerLevel;
        PreferredTurnOrder = preferredTurnOrder;
        Name = enemyName ?? "Missing Name";
        DeathEffect = deathEffect;
        IsHasty = isHasty;
        IsUnique = isUnique;
        Animations = animations;
        AIPreferences = aiPreferences;
        ReactionCards = reactionCards != null ? reactionCards : new ReactionCardType[0];
        Description = description != null ? description : "";
        if (_resourceType == null)
            Log.Error($"Null Resource Type for {Name} {enemyId}");
        if (_counterAdjustments == null)
            Log.Error($"Null Counter Adjustments for {Name} {enemyId}");
    }
        
    public Member AsMember(int id)
    {
        var stats = Stats;
        var m = new Member(id, Name, "Enemy", _materialType, TeamType.Enemies, stats, Role, stats.DefaultPrimaryStat(stats));
        m.State.InitResourceAmount(_resourceType, _startingResourceAmount);
        _counterAdjustments.ForEach(c => m.State.Adjust(c.Key, c.Value));
        return m;
    }
    
    public Member SetupMemberState(Member m, BattleState state)
    {
        var ctx = new EffectContext(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.RewardState, 
            state.Members, state.PlayerCardZones, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), 
            state.CreditsAtStartOfBattle, state.Party.Credits, state.Enemies.ToDictionary(x => x.Member.Id, x => (EnemyType)x.Enemy), 
            () => state.GetNextCardId(), new PlayedCardSnapshot[0], new Dictionary<int, Color>(), new Dictionary<int, Sprite>(), true, ReactionTimingWindow.Default);
        m.State.InitResourceAmount(_resourceType, _startingResourceAmount);
        m.State.ApplyPersistentState(
            new EndOfTurnResourceGainPersistentState(new ResourceQuantity { ResourceType = _resourceType.Name, Amount = _resourceGainPerTurn}, m, state.Party));
        _startOfBattleEffects?.ForEach(effect => AllEffects.Apply(effect, ctx.WithReactionTimingContext(effect.FinalReactionTimingWindow)));
        return m;
    }
    
    public IStats Stats => new StatAddends
        {
            ResourceTypes = _resourceType != null 
                ? new[] {_resourceType.WithMax(Math.Max(_resourceType.MaxAmount, _maxResourceAmount))} 
                : Array.Empty<IResourceType>()
        }
        .With(StatType.MaxHP, _maxHp)
        .With(StatType.MaxShield, _maxShield)
        .With(StatType.StartingShield, _startingShield)
        .With(StatType.Attack, _attack)
        .With(StatType.Magic, _magic)
        .With(StatType.Leadership, _leadership)
        .With(StatType.Economy, 0)
        .With(StatType.Armor, _armor)
        .With(StatType.Resistance, _resistance)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.ExtraCardPlays, _cardsPerTurn);
    
    public int GetRewardCredits(float powerLevelFactor)
    {
        var typeFactor = Tier == EnemyTier.Boss ? 4 : 1;
        return Mathf.RoundToInt(PowerLevel * powerLevelFactor * typeFactor);
    }

    public int GetRewardXp(float powerLevelFactor)
    {
        var typeFactor = Tier == EnemyTier.Boss ? 4 : 1;
        return Mathf.RoundToInt(PowerLevel * powerLevelFactor * typeFactor);
    }
}