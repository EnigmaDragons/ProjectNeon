using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private readonly IStats _baseStats;
    private readonly List<IStats> _battleAdditiveMods = new List<IStats>();
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    
    private IStats CurrentStats => _baseStats
        .Plus(_battleAdditiveMods)
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats));

    private readonly Dictionary<string, BattleCounter> _counters = new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);
    private BattleCounter Counter(string name) => _counters[name];
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    
    public MemberState(IStats baseStats)
    {
        _baseStats = baseStats;
        _counters["HP"] = new BattleCounter(StatType.HP, MaxHP, () => MaxHP);
        _counters[StatType.Shield.ToString()] = new BattleCounter(StatType.Shield, 0, () => MaxShield);
        baseStats.ResourceTypes.ForEach(r => _counters[r.Name] = new BattleCounter(r.Name, 0, () => r.MaxAmount));
    }

    public void ApplyTemporaryAdditive(ITemporalState mods)
    {
        _additiveMods.Add(mods);
    }

    public void ApplyAdditiveUntilEndOfBattle(IStats mods)
    {  
        _battleAdditiveMods.Add(mods);
    }

    public int MaxHP => CurrentStats.MaxHP;
    public int MaxShield => CurrentStats.MaxShield;
    public int Attack => CurrentStats.Attack;
    public int Magic => CurrentStats.Magic;
    public float Armor => CurrentStats.Armor;
    public float Resistance => CurrentStats.Resistance;
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;

    public void GainHp(float amount) => ChangeHp(amount);
    public void GainShield(float amount) => Counter(StatType.Shield).ChangeBy(amount);
    public void TakePhysicalDamage(float amount) => ChangeHp((-(amount * ((1f - Armor)/1f))));
    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => _additiveMods.RemoveAll(condition);
    public void GainResource(string resourceName, int amount) => Counter(resourceName).ChangeBy(amount);

    // @todo #1:15min In The Battle Wrap Up Phase, Advance Turn on all members
    public void AdvanceTurn()
    {
        _additiveMods.ForEach(m => m.AdvanceTurn());
        _additiveMods.RemoveAll(m => !m.IsActive);
    }

    private void ChangeHp(float amount) => Counter(StatType.HP).ChangeBy(amount);
}
