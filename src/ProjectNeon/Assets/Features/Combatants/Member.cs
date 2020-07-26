using System;
using UnityEngine;

public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public MemberState State { get; }
    
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
    public static int CurrentHp(this Member m) => Mathf.CeilToInt(m.State[TemporalStatType.HP]);
    public static bool IsConscious(this Member m) => m.State.IsConscious;

    public static bool CanAfford(this Member m, Card c)
    {
        var cost = c.ResourcesSpent(m);
        return m.State[cost.ResourceType] >= cost.Amount;
    }
}
