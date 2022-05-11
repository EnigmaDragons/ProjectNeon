using System;
using System.Linq;

public static class EffectResolvedExtensions
{
    public static T[] Select<T>(this EffectResolved e, Func<BattleStateSnapshot, T> selector)
        => new[] {selector(e.BattleBefore), selector(e.BattleAfter)};
    public static T[] Select<T>(this EffectResolved e, Member possessor, Func<MemberSnapshot, T> selector)
        => new[] {selector(e.BattleBefore.Members[possessor.Id]), selector(e.BattleAfter.Members[possessor.Id])};
    public static int [] SelectSum(this EffectResolved e, Func<MemberSnapshot, int> selector)
    {
        var targetMembers = e.Target.Members;
        var before = targetMembers
            .Where(t => e.BattleBefore.Members.ContainsKey(t.Id))
            .Select(t => e.BattleBefore.Members[t.Id])
            .Sum(selector);
        var after = targetMembers
            .Where(t => e.BattleBefore.Members.ContainsKey(t.Id))
            .Select(t => e.BattleAfter.Members[t.Id])
            .Sum(selector);
        return new[] {before, after};
    }
}
