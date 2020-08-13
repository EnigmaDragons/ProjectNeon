using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private readonly IStats _baseStats;
    private readonly List<IStats> _battleAdditiveMods = new List<IStats>();
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    private readonly List<ITemporalState> _multiplierMods = new List<ITemporalState>();
    private readonly List<ITemporalState> _reactiveStates = new List<ITemporalState>();

    public IStats BaseStats => _baseStats;
    
    private IStats CurrentStats => _baseStats
        .Plus(_battleAdditiveMods)
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats))
        .Times(_multiplierMods.Where(x => x.IsActive).Select(x => x.Stats));

    private readonly Dictionary<string, BattleCounter> _counters = new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);
    private BattleCounter Counter(string name) => _counters.VerboseGetValue(name, n => $"Counter {n}");
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.ToString()];

    public MemberState(int id, IStats baseStats)
        : this(id, baseStats, baseStats.MaxHP()) {}
    
    public MemberState(int id, IStats baseStats, int initialHp)
    {
        MemberId = id;
        _baseStats = baseStats;
        _counters[TemporalStatType.HP.ToString()] = new BattleCounter(TemporalStatType.HP, initialHp, () => CurrentStats.MaxHP());
        _counters[TemporalStatType.Shield.ToString()] = new BattleCounter(TemporalStatType.Shield, 0, () => CurrentStats.Toughness() * 2);
        _counters[TemporalStatType.TurnStun.ToString()] = new BattleCounter(TemporalStatType.TurnStun, 0, () => int.MaxValue);
        _counters[TemporalStatType.CardStun.ToString()] = new BattleCounter(TemporalStatType.CardStun, 0, () => int.MaxValue);

        baseStats.ResourceTypes?.ForEach(r => _counters[r.Name] = new BattleCounter(r.Name, r.StartingAmount, () => r.MaxAmount));
        _counters["None"] = new BattleCounter("None", 0, () => 0);
    }

    public void InitResourceAmount(IResourceType resourceType, int amount) => _counters[resourceType.Name].Set(amount);

    public int MemberId { get; }
    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => _counters[resourceType.Name].Amount;
    public int ResourceAmount(string resourceType) => _counters[resourceType].Amount;
    public float this[StatType statType] => CurrentStats[statType];
    public float this[TemporalStatType statType] => _counters[statType.ToString()].Amount + CurrentStats[statType];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
    public float Max(string name) => _counters[name].Max;

    public void ApplyTemporaryAdditive(ITemporalState mods) => _additiveMods.Add(mods);
    public void ApplyAdditiveUntilEndOfBattle(IStats mods) => _battleAdditiveMods.Add(mods);
    public void ApplyTemporaryMultiplier(ITemporalState mods) => _multiplierMods.Add(mods);
    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => _additiveMods.RemoveAll(condition);
    public void AddReactiveState(ITemporalState state) => _reactiveStates.Add(state);
    public void RemoveReactiveState(ITemporalState state) => _reactiveStates.Remove(state);

    public void Gain(ResourceQuantity qty) => GainResource(qty.ResourceType, qty.Amount);
    public void Lose(ResourceQuantity qty) => LoseResource(qty.ResourceType, qty.Amount);
    public void GainResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(amount));
    public void LoseResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(-amount));
    public void GainHp(float amount) => ChangeHp(amount);
    public void GainShield(float amount) => PublishAfter(() => Counter(TemporalStatType.Shield).ChangeBy(amount));
    public void GainArmor(float amount) => ApplyAdditiveUntilEndOfBattle(new StatAddends().With(StatType.Armor, amount));
    public void TakeRawDamage(int amount) => ChangeHp(-amount * CurrentStats.Damagability());
    public void TakeDamage(int amount)
    {
        var shieldModificationAmount = Math.Min(amount, Counter(TemporalStatType.Shield).Amount);
        amount -= shieldModificationAmount;
        GainShield(-shieldModificationAmount);
        if (amount > 0)
            ChangeHp(-amount);
    }
    public void ChangeHp(float amount) => Counter(TemporalStatType.HP).ChangeBy(amount);
    public void GainPrimaryResource(int numToGive) => _counters[PrimaryResource.Name].ChangeBy(numToGive);
    public void SpendPrimaryResource(int numToGive) => _counters[PrimaryResource.Name].ChangeBy(-numToGive);
    public int PrimaryResourceAmount => _counters[PrimaryResource.Name].Amount;
    private IResourceType PrimaryResource => ResourceTypes[0];

    public void OnTurnStart()
    {
        _additiveMods.ForEach(m => m.OnTurnStart());
        _multiplierMods.ForEach(m => m.OnTurnStart());
        _reactiveStates.ForEach(m => m.OnTurnStart());
    }
    
    public void OnTurnEnd() => PublishAfter(() =>
    {
        _additiveMods.ForEach(m => m.OnTurnEnd());
        _additiveMods.RemoveAll(m => !m.IsActive);
        _multiplierMods.ForEach(m => m.OnTurnEnd());
        _multiplierMods.RemoveAll(m => !m.IsActive);
        _reactiveStates.ForEach(m => m.OnTurnEnd());
        _reactiveStates.RemoveAll(m => !m.IsActive);
    });

    private void PublishAfter(Action action)
    {
        action();
        Message.Publish(new MemberStateChanged(this));
    }
}
