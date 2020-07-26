
public static class BattleStateExtensions
{
    public static bool IsPlayable(this Card c, BattleState b)
    {
        if (c.LimitedToClass.IsMissing)
            return false;
        
        var member = b.GetMemberByClass(c.LimitedToClass.Value);
        if (!member.IsConscious())
            return false;

        return member.CanAfford(c);
    }
}
