using System;
using System.IO;
using UnityEngine;

public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public BattleRole BattleRole { get; }
    public MemberState State { get; }

    public override string ToString() => $"{Name} {Id}";
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats, BattleRole battleRole)
        : this(id, name, characterClass, team, baseStats, battleRole, baseStats.MaxHp()) {}
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats, BattleRole battleRole, int initialHp)
    {
        if (id > -1 && baseStats.Damagability() < 0.01)
            throw new InvalidDataException($"Damagability of {name} is 0");
        
        Id = id;
        Name = name;
        Class = characterClass;
        TeamType = team;
        BattleRole = battleRole;
        State = new MemberState(id, baseStats, initialHp);
    }

    public void Apply(Action<MemberState> effect)
    {
        effect(State);
    }
}

public static class MemberExtensions
{
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
    
    public static MemberSnapshot GetSnapshot(this Member m) => new MemberSnapshot(m.Id, m.Name, m.Class, m.TeamType, m.State.ToSnapshot());
    public static int CurrentHp(this Member m) => RoundUp(m.State[TemporalStatType.HP]);
    public static int MaxHp(this Member m) => RoundUp(m.State.MaxHp());
    public static int RemainingShieldCapacity(this Member m) => m.MaxShield() - m.CurrentHp(); 
    public static int CurrentShield(this Member m) => RoundUp(m.State[TemporalStatType.Shield]);
    public static int MaxShield(this Member m) => RoundUp(m.State[StatType.Toughness] * 2);
    public static int Attack(this Member m) => m.State.Attack();
    public static int Magic(this Member m) => m.State.Magic();
    public static int Armor(this Member m) => m.State.Armor();
    public static int Resistance(this Member m) => m.State.Resistance();
    public static bool IsConscious(this Member m) => m.State.IsConscious;
    public static bool IsStunnedForCurrentTurn(this Member m) => m.State[TemporalStatType.TurnStun] > 0;
    public static bool IsStunnedForCard(this Member m) => m.State[TemporalStatType.CardStun] > 0;
    public static bool HasAttackBuff(this Member m) => m.State.DifferenceFromBase(StatType.Attack) > 0;
    public static bool HasMaxPrimaryResource(this Member m) => m.State.PrimaryResourceAmount == m.ResourceMax(m.State.PrimaryResource);
    public static int ResourceMax(this Member m, IResourceType resourceType) => RoundUp(m.State.Max(resourceType.Name));

    public static bool CanAfford(this Member m, ResourceCost c)
    {
        if (!c.IsXCost && c.Amount == 0)
            return true;
        var costAmount = c.ResourcesSpent(m);
        var remaining = m.State.ResourceAmount(costAmount.ResourceType) - costAmount.Amount;
        return remaining >= 0;
    }
}
