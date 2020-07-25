using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private readonly IStats _baseStats;
    private readonly List<IStats> _battleAdditiveMods = new List<IStats>();
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    private readonly List<ITemporalState> _multiplierMods = new List<ITemporalState>();

    private IStats CurrentStats => _baseStats
        .Plus(_battleAdditiveMods)
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats))
        .Times(_multiplierMods.Where(x => x.IsActive).Select(x => x.Stats));

    private readonly Dictionary<string, BattleCounter> _counters = new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);
    private readonly Dictionary<string, string> _status = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
    private BattleCounter Counter(string name) => _counters.VerboseGetValue(name, n => $"Counter {n}");
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.ToString()];

    public MemberState(IStats baseStats)
    {
        _baseStats = baseStats;
        _counters["HP"] = new BattleCounter(TemporalStatType.HP, _baseStats.MaxHP(), () => CurrentStats.MaxHP());
        _counters[TemporalStatType.Shield.ToString()] = new BattleCounter(TemporalStatType.Shield, 0, () => CurrentStats.Toughness() * 2);

        baseStats.ResourceTypes?.ForEach(r => _counters[r.Name] = new BattleCounter(r.Name, r.StartingAmount, () => r.MaxAmount));
        _counters["None"] = new BattleCounter("None", 0, () => 0);
    }

    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => _counters[resourceType.Name].Amount;
    public float this[StatType statType] => CurrentStats[statType];
    public float this[TemporalStatType temporalStatType] => _counters[temporalStatType.ToString()].Amount;
    public string this[string status] => _status[status];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;

    public void ApplyTemporaryAdditive(ITemporalState mods) => _additiveMods.Add(mods);
    public void ApplyAdditiveUntilEndOfBattle(IStats mods) => _battleAdditiveMods.Add(mods);
    public void ApplyTemporaryMultiplier(ITemporalState mods) => _multiplierMods.Add(mods);
    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => _additiveMods.RemoveAll(condition);

    public void GainResource(string resourceName, int amount) => Counter(resourceName).ChangeBy(amount);
    public void LoseResource(string resourceName, int amount) => Counter(resourceName).ChangeBy(-amount);
    public void GainHp(float amount) => ChangeHp(amount);
    public void GainShield(float amount) => Counter(TemporalStatType.Shield).ChangeBy(amount);
    public void GainArmor(float amount) => ApplyAdditiveUntilEndOfBattle(new StatAddends().With(StatType.Armor, amount));
    public void TakeRawDamage(int amount) => ChangeHp(-amount * CurrentStats.Damagability());
    public void ChangeHp(float amount) => Counter(TemporalStatType.HP).ChangeBy(amount);
    public void GainPrimaryResource(int numToGive) => _counters[PrimaryResource.Name].ChangeBy(numToGive);
    public void SpendPrimaryResource(int numToGive) => _counters[PrimaryResource.Name].ChangeBy(-numToGive);
    public int PrimaryResourceAmount => _counters[PrimaryResource.Name].Amount;
    private IResourceType PrimaryResource => ResourceTypes[0];

    public void ChangeStatus(string status, string value)
    {
        _status[status] = value;
    }

    public void Stun(int duration)
    {
        if (!_counters.ContainsKey("Stun"))
        {
            _counters["Stun"] = new BattleCounter(TemporalStatType.Stun, 0, () => 0);
        }
        Counter(TemporalStatType.Stun).Set(duration);
    }

    // @todo #380:30min Stun effect is created, but it does nothing. Implement Styun behaviort so a character
    //  with the Stun TemporalStat won't be able to play a card in the current turn.

    public void AdvanceTurn()
    {
        _additiveMods.ForEach(m => m.AdvanceTurn());
        _additiveMods.RemoveAll(m => !m.IsActive);
    }
}
