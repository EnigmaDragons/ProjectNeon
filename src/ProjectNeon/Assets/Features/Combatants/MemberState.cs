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
    private BattleCounter Counter(string name) => _counters[name];
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.ToString()];

    public MemberState(IStats baseStats)
    {
        _baseStats = baseStats;
        _counters["HP"] = new BattleCounter(TemporalStatType.HP, _baseStats.MaxHP(), () => CurrentStats.MaxHP());
        _counters[TemporalStatType.Shield.ToString()] = new BattleCounter(TemporalStatType.Shield, 0, () => CurrentStats.Toughness() * 2);
        baseStats.ResourceTypes.ForEach(r => _counters[r.Name] = new BattleCounter(r.Name, 0, () => r.MaxAmount));
    }

    public float this[StatType statType] => CurrentStats[statType];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
    
    public void ApplyTemporaryAdditive(ITemporalState mods) => _additiveMods.Add(mods);
    public void ApplyAdditiveUntilEndOfBattle(IStats mods) => _battleAdditiveMods.Add(mods);
    public void ApplyTemporaryMultiplier(ITemporalState mods) => _multiplierMods.Add(mods);
    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => _additiveMods.RemoveAll(condition);

    public void GainResource(string resourceName, int amount) => Counter(resourceName).ChangeBy(amount);
    public void GainHp(float amount) => ChangeHp(amount);
    public void GainShield(float amount) => Counter(TemporalStatType.Shield).ChangeBy(amount);
    public void GainArmor(float amount) => Counter(TemporalStatType.Armor).ChangeBy(amount);
    public void TakeRawDamage(int amount) => ChangeHp(-amount * CurrentStats.Damagability());
    public void TakePhysicalDamage(float amount) => ChangeHp((-(amount * ((1f - CurrentStats.Armor()) / 1f))) * CurrentStats.Damagability());
    private void ChangeHp(float amount) => Counter(TemporalStatType.HP).ChangeBy(amount);

    // @todo #1:15min In The Battle Wrap Up Phase, Advance Turn on all members
    public void AdvanceTurn()
    {
        _additiveMods.ForEach(m => m.AdvanceTurn());
        _additiveMods.RemoveAll(m => !m.IsActive);
    }
}
