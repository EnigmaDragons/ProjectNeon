using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MemberState : IStats
{
    private IStats _baseStats;
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    private IStats CurrentStats => _baseStats.Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats).ToArray());
    private int _hp;
    private int _shield;
    private int[] _resources;
    private int[] _resourcesMax;
    
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    public int Shield
    {
        get => _shield;
        set => _shield = value;
    }

    public int Resource1 { get; set; }
    public int Resource2 { get; set; }
    
    public MemberState(IStats baseStats)
    {
        _baseStats = baseStats;
        _hp = baseStats.MaxHP;
        _shield = 0;
        _resources = new int[baseStats.ResourceTypes.Length];
        _resourcesMax = baseStats.ResourceTypes.Select(x => x.MaxAmount).ToArray();
    }

    public void ApplyTemporaryAdditive(ITemporalState mods)
    {
        _additiveMods.Add(mods);
    }

    public void ApplyUntilEndOfBattle(BattleStats mods)
    {
        mods.Init(CurrentStats);
        _baseStats = mods;
    }

    public int MaxHP => CurrentStats.MaxHP;
    public int MaxShield => CurrentStats.MaxShield;
    public int Attack => CurrentStats.Attack;
    public int Magic => CurrentStats.Magic;
    public float Armor => CurrentStats.Armor;
    public float Resistance => CurrentStats.Resistance;
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;

    public void GainHp(float amount) => ChangeHp(amount);
    public void TakePhysicalDamage(float amount) => ChangeHp((-(amount * ((1f - Armor)/1f))));

    // @todo #1:15min In The Battle Wrap Up Phase, Advance Turn on all members
    public void AdvanceTurn()
    {
        _additiveMods.ForEach(m => m.AdvanceTurn());
        _additiveMods.RemoveAll(m => !m.IsActive);
    }

    private void ChangeHp(float amount)
    {
        HP = RoundUp(Mathf.Clamp(HP + amount, 0, MaxHP));
    }

    private int RoundUp(float amount) => Convert.ToInt32(Math.Ceiling(amount));
}
