using System;
using System.Linq;

public interface Target  
{
    Member[] Members { get; }
}

public static class TargetExtensions
{
    public static void ApplyToAll(this Target target, Action<MemberState> effect)
    {
        target.Members.ForEach(m => m.Apply(effect));
    }

    public static string MembersDescriptions(this Target target) =>
        string.Join(", ", target.Members.Select(m => $"{m.Name} {m.Id}"));
}
