using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public BattleRole BattleRole { get; }
    public MemberState State { get; }
    public Maybe<CardTypeData> BasicCard { get; }

    public override bool Equals(object obj) => obj is Member && ((Member)obj).Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => $"{Name} {Id}";
    
    public string UnambiguousName => TeamType == TeamType.Enemies ? ToString() : Name;
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats, BattleRole battleRole, StatType primaryStat)
        : this(id, name, characterClass, team, baseStats, battleRole, primaryStat, baseStats.MaxHp(), Maybe<CardTypeData>.Missing()) {}
    
    public Member(int id, string name, string characterClass, TeamType team, IStats baseStats, BattleRole battleRole, StatType primaryStat, int initialHp, Maybe<CardTypeData> basicCard)
    {
        if (id > -1 && baseStats.Damagability() < 0.01)
            throw new InvalidDataException($"Damagability of {name} is 0");
        
        Id = id;
        Name = name;
        Class = characterClass;
        TeamType = team;
        BattleRole = battleRole;
        State = new MemberState(id, name, baseStats, primaryStat, initialHp);
        BasicCard = basicCard;
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

    public static string Names(this IEnumerable<Member> members) => string.Join(", ", members.Select(m => m.Name));
    
    public static MemberSnapshot GetSnapshot(this Member m) => new MemberSnapshot(m.Id, m.Name, m.Class, m.TeamType, m.State.ToSnapshot());
    public static int CurrentHp(this Member m) => RoundUp(m.State[TemporalStatType.HP]);
    public static int MaxHp(this Member m) => RoundUp(m.State.MaxHp());
    public static int MissingHp(this Member m) => RoundUp(m.State.MaxHp() - m.CurrentHp());
    public static int RemainingShieldCapacity(this Member m) => m.MaxShield() - m.CurrentShield(); 
    public static int CurrentShield(this Member m) => RoundUp(m.State[TemporalStatType.Shield]);
    public static int MaxShield(this Member m) => RoundUp(m.State.MaxShield());
    public static int Attack(this Member m) => m.State.Attack();
    public static int ExtraCardPlays(this Member m) => RoundUp(m.State[StatType.ExtraCardPlays]);
    public static int Magic(this Member m) => m.State.Magic();
    public static int Leadership(this Member m) => m.State.Leadership();
    public static int Economic(this Member m) => m.State.Economy();
    public static int Armor(this Member m) => m.State.Armor();
    public static int Dodge(this Member m) => RoundUp(m.State[TemporalStatType.Dodge]);
    public static int Aegis(this Member m) => RoundUp(m.State[TemporalStatType.Aegis]);
    public static int Taunt(this Member m) => RoundUp(m.State[TemporalStatType.Taunt]);
    public static int Stealth(this Member m) => RoundUp(m.State[TemporalStatType.Stealth]);
    public static int Resistance(this Member m) => m.State.Resistance();
    public static bool CanPlayCards(this Member m) => m.IsConscious() && !m.IsDisabled();
    public static bool IsConscious(this Member m) => m.State.IsConscious;
    public static bool IsUnconscious(this Member m) => m.State.IsUnconscious;
    public static bool IsVulnerable(this Member m) => m.State[TemporalStatType.Vulnerable] > 1;
    public static bool IsDisabled(this Member m) => m.State[TemporalStatType.Disabled] > 0;
    public static bool IsStunnedForCard(this Member m) => m.State[TemporalStatType.Stun] > 0;
    public static bool IsBlinded(this Member m) => m.State[TemporalStatType.Blind] > 0;
    public static bool IsInhibited(this Member m) => m.State[TemporalStatType.Inhibit] > 0;
    public static bool CanReact(this Member m) => !m.IsDisabled() && !m.IsStunnedForCard();
    public static bool HasAttackBuff(this Member m) => m.State.DifferenceFromBase(StatType.Attack) > 0;
    public static bool HasDoubleDamage(this Member m) => m.State[TemporalStatType.DoubleDamage] > 0;
    public static bool HasTaunt(this Member m) => m.State[TemporalStatType.Taunt] > 0;
    public static bool IsAfflicted(this Member m) => m.State.StatusesOfType(StatusTag.DamageOverTime).Length > 0;
    public static bool IsBloodied(this Member m) => m.IsConscious() && m.CurrentHp() * 2 <= m.MaxHp();
    public static bool IsStealthed(this Member m) => m.State[TemporalStatType.Stealth] > 0;
    public static bool IsProminent(this Member m) => m.State[TemporalStatType.Prominent] > 0;
    public static bool IsConfused(this Member m) => m.State[TemporalStatType.Confused] > 0;
    public static bool IsMarked(this Member m) => m.State[TemporalStatType.Marked] > 0;
    public static bool HasMaxPrimaryResource(this Member m) => m.State.PrimaryResourceAmount == m.ResourceMax(m.State.PrimaryResource);
    public static int PrimaryResourceAmount(this Member m) => m.State.PrimaryResourceAmount;
    public static ResourceQuantity PrimaryResource(this Member m) => m.State.CurrentPrimaryResources;
    public static StatType PrimaryStat(this Member m) => m.State.PrimaryStat;
    
    public static int RemainingPrimaryResourceCapacity(this Member m) => m.ResourceMax(m.State.PrimaryResource) - m.State.PrimaryResourceAmount;
    public static int ResourceMax(this Member m, IResourceType resourceType) => RoundUp(m.State.Max(resourceType.Name));
    public static int ResourceAmount(this Member m, IResourceType resourceType) => RoundUp(m.State[resourceType]);

    public static bool CanAfford(this Member m, CardTypeData c, PartyAdventureState partyState)
    {
        try
        {
            if (!c.Cost.PlusXCost && c.Cost.BaseAmount == 0)
                return true;
            var calc = m.CalculateResources(c);
            var amountAvailable = c.Cost.ResourceType.Name == "Creds" ? partyState.Credits : m.State.ResourceAmount(calc.ResourcePaidType.Name);
            var remaining = amountAvailable - calc.ResourcesPaid;
            return remaining >= 0;
        }
        catch (Exception)
        {
            Debug.Log($"{c.Name} has something null");
            throw;
        }
    }

    public static ResourceCalculations CalculateResources(this Member m, CardTypeData card)
        => m.State.CalculateResources(card).ClampResources(m);
    
    //Should reaction cards be modified by changers
    public static ResourceCalculations CalculateResourcesForReaction(this Member m, ReactionCardType reaction)
        => TimelessResourceCalculator.CalculateResources(reaction.Cost, new InMemoryResourceAmount(0), m.State).ClampResources(m);
}
