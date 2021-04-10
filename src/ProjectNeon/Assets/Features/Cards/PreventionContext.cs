
using System.Linq;

public interface PreventionContext
{
    void RecordPreventionTypeEffect(PreventionType type, Member[] members);
    Member[] GetPreventingMembers(PreventionType type);
    void UpdatePreventionCounters();
}

public static class PreventionContextExtensions
{
    public static bool IsDodging(this PreventionContext ctx, Member m) =>
        ctx.GetPreventingMembers(PreventionType.Dodge).Any(p => p.Id == m.Id);
    public static bool IsAegising(this PreventionContext ctx, Member m) =>
        ctx.GetPreventingMembers(PreventionType.Aegis).Any(p => p.Id == m.Id);
}
