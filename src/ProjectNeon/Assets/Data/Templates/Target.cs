using System;

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
}
