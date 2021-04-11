using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyInstance
{
    private readonly ResourceType _resourceType;
    private readonly EffectData[] _startOfBattleEffects;
    private readonly int _startingResourceAmount;
    private readonly int _resourceGainPerTurn;
    private readonly int _maxResourceAmount;
    private readonly int _maxHp;
    private readonly int _maxShield;
    private readonly int _startingShield;
    private readonly int _toughness;
    private readonly int _attack;
    private readonly int _magic;
    private readonly int _leadership;
    private readonly float _armor;
    private readonly float _resistance;
    private readonly int _cardsPerTurn;

    public GameObject Prefab { get; }
    public TurnAI AI { get; }
    public IEnumerable<CardType> Cards { get; }
    public BattleRole Role { get; }
    public EnemyTier Tier { get; }
    public int PowerLevel { get; }
    public int PreferredTurnOrder { get; }
    public string Name { get; }
    public string DeathEffect { get; }
    public bool IsHasty { get; }
    public bool IsUnique { get; }
    
    public bool DeckIsValid => Cards.None(x => x == null);
    public bool IsReadyForPlay => Cards != null && Prefab != null && AI != null;

    public EnemyInstance(ResourceType resourceType, EffectData[] startOfBattleEffects, int startingResourceAmount, int resourceGainPerTurn, int maxResourceAmount, int maxHp, int maxShield, int startingShield, int toughness, int attack, int magic, int leadership, float armor, float resistance, int cardsPerTurn, GameObject prefab, TurnAI ai, IEnumerable<CardType> cards, BattleRole role, EnemyTier tier, int powerLevel, int preferredTurnOrder, string name, string deathEffect, bool isHasty, bool isUnique)
    {
        _resourceType = resourceType;
        _startOfBattleEffects = startOfBattleEffects;
        _startingResourceAmount = startingResourceAmount;
        _resourceGainPerTurn = resourceGainPerTurn;
        _maxResourceAmount = maxResourceAmount;
        _maxHp = maxHp;
        _maxShield = maxShield;
        _startingShield = startingShield;
        _toughness = toughness;
        _attack = attack;
        _magic = magic;
        _leadership = leadership;
        _armor = armor;
        _resistance = resistance;
        _cardsPerTurn = cardsPerTurn;
        Prefab = prefab;
        AI = ai;
        Cards = cards;
        Role = role;
        Tier = tier;
        PowerLevel = powerLevel;
        PreferredTurnOrder = preferredTurnOrder;
        Name = name;
        DeathEffect = deathEffect;
        IsHasty = isHasty;
        IsUnique = isUnique;
    }
        
    public Member AsMember(int id)
    {
        var stats = Stats;
        var m = new Member(id, Name, "Enemy", TeamType.Enemies, stats, Role, stats.PrimaryStat(stats));
        return m;
    }
    
    public Member SetupMemberState(Member m, BattleState state)
    {
        var ctx = new EffectContext(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, 
            state.Members, state.PlayerCardZones, new UnpreventableContext());
        m.State.InitResourceAmount(_resourceType, _startingResourceAmount);
        m.State.ApplyPersistentState(
            new EndOfTurnResourceGainPersistentState(new ResourceQuantity { ResourceType = _resourceType.Name, Amount = _resourceGainPerTurn}, m));
        _startOfBattleEffects?.ForEach(effect => AllEffects.Apply(effect, ctx));
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
        .With(StatType.Toughness, _toughness)
        .With(StatType.Attack, _attack)
        .With(StatType.Magic, _magic)
        .With(StatType.Leadership, _leadership)
        .With(StatType.Armor, _armor)
        .With(StatType.Resistance, _resistance)
        .With(StatType.Damagability, 1f)
        .With(StatType.Healability, 1f)
        .With(StatType.ExtraCardPlays, _cardsPerTurn);
    
    public int GetRewardCredits(float powerLevelFactor)
    {
        var typeFactor = Role == BattleRole.Boss ? 4 : 1;
        return Mathf.RoundToInt(PowerLevel * powerLevelFactor * typeFactor);
    }

    public int GetRewardXp(float powerLevelFactor)
    {
        var typeFactor = Role == BattleRole.Boss ? 4 : 1;
        return Mathf.RoundToInt(PowerLevel * powerLevelFactor * typeFactor);
    }
}