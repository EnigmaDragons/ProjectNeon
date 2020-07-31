using System;
using UnityEngine;

public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public MemberState State { get; }

    public override string ToString() => $"{Name} {Id}";
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats)
    {
        if (baseStats.Damagability() < 0.01)
            Debug.LogWarning($"Damagability of {name} is 0");
        
        Id = id;
        Name = name;
        Class = characterClass;
        TeamType = team;
        State = new MemberState(id, baseStats);
    }

    public void Apply(Action<MemberState> effect)
    {
        effect(State);
    }
}

public static class MemberExtensions
{
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
    
    public static int CurrentHp(this Member m) => RoundUp(m.State[TemporalStatType.HP]);
    public static int MaxHp(this Member m) => RoundUp(m.State.MaxHP());
    public static int CurrentShield(this Member m) => RoundUp(m.State[TemporalStatType.Shield]);
    public static int MaxShield(this Member m) => RoundUp(m.State[StatType.Toughness] * 2); 
    public static bool IsConscious(this Member m) => m.State.IsConscious;
    public static int ResourceMax(this Member m, IResourceType resourceType) => RoundUp(m.State.Max(resourceType.Name));

    public static bool CanAfford(this Member m, CardType c)
    {
        if (!c.Cost.IsXCost && c.Cost.Amount == 0)
            return true;
        var cost = c.ResourcesSpent(m);
        var remaining = m.State[cost.ResourceType] - cost.Amount;
        return remaining >= 0;
    }
}
