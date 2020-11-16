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

    public override bool Equals(object obj) => obj is Member && ((Member)obj).Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
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

    public Member Apply(Action<MemberState> effect)
    {
        effect(State);
        return this;
    }
}

public static class MemberExtensions
{
    private static int RoundUp(float v) => Mathf.CeilToInt(v);
    
    public static MemberSnapshot GetSnapshot(this Member m) => new MemberSnapshot(m.Id, m.Name, m.Class, m.TeamType, m.State.ToSnapshot());
    public static int CurrentHp(this Member m) => RoundUp(m.State[TemporalStatType.HP]);
    public static int MaxHp(this Member m) => RoundUp(m.State.MaxHp());
    public static int RemainingShieldCapacity(this Member m) => m.MaxShield() - m.CurrentShield(); 
    public static int CurrentShield(this Member m) => RoundUp(m.State[TemporalStatType.Shield]);
    public static int MaxShield(this Member m) => RoundUp(m.State[StatType.Toughness] * 2);
    public static int Attack(this Member m) => m.State.Attack();
    public static int Magic(this Member m) => m.State.Magic();
    public static int Leadership(this Member m) => m.State.Leadership();
    public static int Armor(this Member m) => m.State.Armor();
    public static int Resistance(this Member m) => m.State.Resistance();
    public static int Toughness(this Member m) => m.State.Toughness();
    public static bool CanPlayCards(this Member m) => m.IsConscious() && !m.IsStunnedForCurrentTurn();
    public static bool IsConscious(this Member m) => m.State.IsConscious;
    public static bool IsVulnerable(this Member m) => m.State.Damagability() > 1;
    public static bool IsStunnedForCurrentTurn(this Member m) => m.State[TemporalStatType.TurnStun] > 0;
    public static bool IsStunnedForCard(this Member m) => m.State[TemporalStatType.CardStun] > 0;
    public static bool HasAttackBuff(this Member m) => m.State.DifferenceFromBase(StatType.Attack) > 0;
    public static bool HasDoubleDamage(this Member m) => m.State[TemporalStatType.DoubleDamage] > 0;
    public static bool HasTaunt(this Member m) => m.State[TemporalStatType.Taunt] > 0;
    public static bool IsStealth(this Member m) => m.State[TemporalStatType.Stealth] > 0;
    public static bool IsConfused(this Member m) => m.State[TemporalStatType.Confusion] > 0;
    public static bool HasMaxPrimaryResource(this Member m) => m.State.PrimaryResourceAmount == m.ResourceMax(m.State.PrimaryResource);
    public static int PrimaryResourceAmount(this Member m) => m.State.PrimaryResourceAmount;
    
    public static int RemainingPrimaryResourceCapacity(this Member m) => m.ResourceMax(m.State.PrimaryResource) - m.State.PrimaryResourceAmount;
    public static int ResourceMax(this Member m, IResourceType resourceType) => RoundUp(m.State.Max(resourceType.Name));
    public static int ResourceAmount(this Member m, IResourceType resourceType) => RoundUp(m.State[resourceType]);

    public static bool CanAfford(this Member m, ResourceCost c)
    {
        if (!c.PlusXCost && c.BaseAmount == 0)
            return true;
        var costAmount = c.ResourcesSpent(m);
        var remaining = m.State.ResourceAmount(costAmount.ResourceType) - costAmount.Amount;
        return remaining >= 0;
    }
}
