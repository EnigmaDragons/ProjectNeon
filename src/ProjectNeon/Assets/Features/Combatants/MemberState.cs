using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private int _versionNumber;
    private readonly Dictionary<string, BattleCounter> _counters = new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);
    
    private readonly IStats _baseStats;
    private readonly List<IPersistentState> _persistentStates = new List<IPersistentState>();
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    private readonly List<ITemporalState> _multiplierMods = new List<ITemporalState>();
    private readonly List<ReactiveStateV2> _reactiveStates = new List<ReactiveStateV2>();

    public IStats BaseStats => _baseStats;
    
    private IStats CurrentStats => _baseStats
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats))
        .Times(_multiplierMods.Where(x => x.IsActive).Select(x => x.Stats))
        .NotBelow(0);

    private BattleCounter Counter(string name) => _counters.VerboseGetValue(name, n => $"Counter '{n}'");
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.ToString()];

    public int MemberId { get; }
    
    public MemberState(int id, IStats baseStats)
        : this(id, baseStats, baseStats.MaxHp()) {}
    
    public MemberState(int id, IStats baseStats, int initialHp)
    {
        MemberId = id;
        _baseStats = baseStats;
        _counters[TemporalStatType.HP.ToString()] = new BattleCounter(TemporalStatType.HP, initialHp, () => CurrentStats.MaxHp());
        _counters[TemporalStatType.Shield.ToString()] = new BattleCounter(TemporalStatType.Shield, 0, () => CurrentStats.Toughness() * 2);
        _counters[TemporalStatType.TurnStun.ToString()] = new BattleCounter(TemporalStatType.TurnStun, 0, () => int.MaxValue);
        _counters[TemporalStatType.CardStun.ToString()] = new BattleCounter(TemporalStatType.CardStun, 0, () => int.MaxValue);
        _counters[TemporalStatType.Evade.ToString()] = new BattleCounter(TemporalStatType.Evade, 0, () => int.MaxValue);
        _counters[TemporalStatType.Spellshield.ToString()] = new BattleCounter(TemporalStatType.Evade, 0, () => int.MaxValue);
        _counters[TemporalStatType.Taunt.ToString()] = new BattleCounter(TemporalStatType.Taunt, 0, () => int.MaxValue);
        _counters[TemporalStatType.Stealth.ToString()] = new BattleCounter(TemporalStatType.Stealth, 0, () => int.MaxValue);
        _counters[TemporalStatType.DoubleDamage.ToString()] = new BattleCounter(TemporalStatType.DoubleDamage, 0, () => int.MaxValue);
        baseStats.ResourceTypes?.ForEach(r => _counters[r.Name] = new BattleCounter(r.Name, r.StartingAmount, () => r.MaxAmount));
        _counters["None"] = new BattleCounter("None", 0, () => 0);
        _counters[""] = new BattleCounter("", 0, () => 0);
    }

    public void InitResourceAmount(IResourceType resourceType, int amount) => _counters[resourceType.Name].Set(amount);

    // Queries
    public MemberStateSnapshot ToSnapshot() => new MemberStateSnapshot(_versionNumber, MemberId, CurrentStats, _counters.ToDictionary(c => c.Key, c => c.Value.Amount));
    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => _counters[resourceType.Name].Amount;
    public int ResourceAmount(string resourceType) => _counters[resourceType].Amount;
    public float this[StatType statType] => CurrentStats[statType];
    public float this[TemporalStatType statType] => _counters[statType.ToString()].Amount + CurrentStats[statType];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
    public float Max(string name) => _counters[name].Max;
    public IResourceType PrimaryResource => ResourceTypes[0];
    public int PrimaryResourceAmount => _counters[PrimaryResource.Name].Amount;
    public int DifferenceFromBase(StatType statType) => (CurrentStats[statType] - _baseStats[statType]).CeilingInt();
    public ReactiveStateV2[] ReactiveStates => _reactiveStates.ToArray();
    public bool HasStatus(StatusTag tag) => _reactiveStates.Any(r => r.Tag == tag) 
                                            || _additiveMods.Any(r => r.Tag == tag) 
                                            || _multiplierMods.Any(r => r.Tag == tag);
    
    // Reaction Commands
    public ProposedReaction[] GetReactions(EffectResolved e) =>
        _reactiveStates
            .Select(x => x.OnEffectResolved(e))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();

    // Modifier Commands
    private static readonly HashSet<StatusTag> NonStackingStatuses = new HashSet<StatusTag> { StatusTag.Vulnerable };

    public void ApplyPersistentState(IPersistentState state) => _persistentStates.Add(state);
    
    public void ApplyTemporaryAdditive(ITemporalState mods) => PublishAfter(() =>
    {
        if (NonStackingStatuses.Contains(mods.Tag))
            _additiveMods.RemoveAll(m => m.Tag == mods.Tag);
        _additiveMods.Add(mods);
    });
    
    public void ApplyTemporaryMultiplier(ITemporalState mods) => PublishAfter(() => 
    {        
        if (NonStackingStatuses.Contains(mods.Tag))
            _multiplierMods.RemoveAll(m => m.Tag == mods.Tag);
        _multiplierMods.Add(mods);
    });
    
    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => PublishAfter(() =>
    {
        _additiveMods.RemoveAll(condition);
        _multiplierMods.RemoveAll(condition);
        _reactiveStates.RemoveAll(condition);
    });
    public void AddReactiveState(ReactiveStateV2 state) 
        => PublishAfter(() =>
        {
            _reactiveStates.RemoveAll(r => r.Tag == state.Tag);
            _reactiveStates.Add(state);
        });

    public void RemoveReactiveState(ReactiveStateV2 state) => PublishAfter(() => _reactiveStates.Remove(state));

    // HP Commands
    public void GainHp(float amount) => ChangeHp(amount);
    public void TakeRawDamage(int amount) => ChangeHp(-amount * CurrentStats.Damagability());
    public void TakeDamage(int amount)
    {
        var shieldModificationAmount = Math.Min(amount, Counter(TemporalStatType.Shield).Amount);
        amount -= shieldModificationAmount;
        GainShield(-shieldModificationAmount);
        if (amount > 0)
            ChangeHp(-amount);
    }
    private void ChangeHp(float amount) => PublishAfter(() => Counter(TemporalStatType.HP).ChangeBy(amount));

    // Status Counters Commands
    public void Adjust(TemporalStatType t, float amount) => PublishAfter(() => Counter(t.ToString()).ChangeBy(amount));
    public void GainShield(float amount) => Adjust(TemporalStatType.Shield, amount);
    public void AdjustEvade(float amount) => Adjust(TemporalStatType.Evade, amount);
    public void AdjustSpellshield(float amount) => Adjust(TemporalStatType.Spellshield, amount);
    public void AdjustDoubleDamage(float amount) => Adjust(TemporalStatType.DoubleDamage, amount);

    // Resource Commands
    public void Gain(ResourceQuantity qty) => GainResource(qty.ResourceType, qty.Amount);
    public void GainResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(amount));
    public void AdjustPrimaryResource(int numToGive) => PublishAfter(() => _counters[PrimaryResource.Name].ChangeBy(numToGive));
    public void Lose(ResourceQuantity qty) => LoseResource(qty.ResourceType, qty.Amount);
    public void LoseResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(-amount));
    public void SpendPrimaryResource(int numToGive) => PublishAfter(() => _counters[PrimaryResource.Name].ChangeBy(-numToGive));

    public void OnTurnStart()
    {
        _persistentStates.ForEach(m => m.OnTurnStart());
        _additiveMods.ForEach(m => m.OnTurnStart());
        _multiplierMods.ForEach(m => m.OnTurnStart());
        _reactiveStates.ForEach(m => m.OnTurnStart());
    }
    
    private readonly List<TemporalStatType> _temporalStatsToReduceAtEndOfTurn = new List<TemporalStatType> { TemporalStatType.Taunt, TemporalStatType.Stealth };
    
    public void OnTurnEnd() => PublishAfter(() =>
    {
        _persistentStates.ForEach(m => m.OnTurnEnd());
        _additiveMods.ForEach(m => m.OnTurnEnd());
        _additiveMods.RemoveAll(m => !m.IsActive);
        _multiplierMods.ForEach(m => m.OnTurnEnd());
        _multiplierMods.RemoveAll(m => !m.IsActive);
        _reactiveStates.ForEach(m => m.OnTurnEnd());
        _reactiveStates.RemoveAll(m => !m.IsActive);
        _temporalStatsToReduceAtEndOfTurn.ForEach(s => _counters[s.ToString()].ChangeBy(-1));
    });

    private void PublishAfter(Action action)
    {
        var before = ToSnapshot();
        _versionNumber++;
        action();
        Message.Publish(new MemberStateChanged(before, this));
    }

}
