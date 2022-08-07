﻿using System;
using System.Linq;

public interface Target  
{
    Member[] Members { get; }
}

public static class TargetExtensions
{
    public static void ApplyToAllConsciousMembers(this Target t, Action<Member> effect)
        => t.Members.Where(x => x.IsConscious()).ForEach(effect);
    
    public static void ApplyToAllConscious(this Target t, Action<MemberState> effect)
        => t.Members.Where(x => x.IsConscious()).ForEach(m => m.Apply(effect));
    
    public static string MembersDescriptions(this Target t) 
        => string.Join(", ", t.Members.Select(m => $"{m.Name} {m.Id}"));

    public static int TotalShields(this Target t) 
        => t.Members.Sum(m => m.CurrentShield());
    
    public static int TotalHpAndShields(this Target t) 
        => t.Members.Sum(m => m.CurrentShield() + m.CurrentHp());

    public static int TotalMissingHp(this Target t)
        => t.Members.Sum(m => m.MaxHp() - m.CurrentHp());

    public static int TotalRemainingShieldCapacity(this Target t)
        => t.Members.Sum(m => m.RemainingShieldCapacity());

    public static int TotalAttack(this Target t)
        => t.Members.Sum(m => m.Attack());

    public static int TotalMagic(this Target t)
        => t.Members.Sum(m => m.Magic());

    public static int TotalOffense(this Target t)
        => t.TotalAttack() + t.TotalMagic();
    
    public static float TotalResourceValue(this Target t) 
        => t.Members.Sum(m => m.State.PrimaryResourceValue);
    
    public static float TotalDodgeValue(this Target t) 
        => t.Members.Sum(m => m.State[TemporalStatType.Dodge]);
    
    public static float TotalResistance(this Target t) 
        => t.Members.Sum(m => m.State[StatType.Resistance]);
    
    public static float TotalArmor(this Target t) 
        => t.Members.Sum(m => m.State[StatType.Armor]);

    public static bool HasShield(this Target t)
        => t.Members.Sum(m => m.CurrentShield()) > 0;
    
    

    public static bool Matches(this Target t, Target other)
        => string.Join(",", t.Members.Select(m => m.Id.GetString()))
            .Equals(string.Join(",", other.Members.Select(m => m.Id.GetString())));
}
