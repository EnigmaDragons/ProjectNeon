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
        State = new MemberState(baseStats);
    }

    public void Apply(Action<MemberState> effect)
    {
        effect(State);
        BattleEvent.Publish(new MemberStateChanged(this));
    }
}
