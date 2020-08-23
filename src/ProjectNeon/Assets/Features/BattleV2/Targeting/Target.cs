using System;
using System.Linq;

public interface Target  
{
    Member[] Members { get; }
}

public static class TargetExtensions
{
    public static void ApplyToAllConscious(this Target t, Action<MemberState> effect)
        => t.Members.Where(x => x.IsConscious()).ForEach(m => m.Apply(effect));
    
    public static string MembersDescriptions(this Target t) 
        => string.Join(", ", t.Members.Select(m => $"{m.Name} {m.Id}"));

    public static int TotalHpAndShields(this Target t) 
        => t.Members.Sum(m => m.CurrentShield() + m.CurrentHp());

    public static int TotalMissingHp(this Target t)
        => t.Members.Sum(m => m.MaxHp() - m.CurrentHp());

    public static int TotalRemainingShieldCapacity(this Target t)
        => t.Members.Sum(m => m.RemainingShieldCapacity());

    public static bool HasShield(this Target t)
        => t.Members.Sum(m => m.CurrentShield()) > 0;

    public static bool Matches(this Target t, Target other)
        => string.Join(",", t.Members.Select(m => m.Id.ToString()))
            .Equals(string.Join(",", other.Members.Select(m => m.Id.ToString())));
}
