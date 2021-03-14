
public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardTypeData c, Member member)
    {
        if (c == null)
        {
            Log.Error($"Null - IsPlayable Check with Null CardTypeData");
            return false;
        }
        if (member == null)
        {
            Log.Error($"Null - IsPlayable Check with Null Member");
            return false;
        }

        if (!member.IsConscious())
            return false;
        if (member.State[TemporalStatType.Disabled] > 0)
            return false;
        return member.CanAfford(c);
    }
    
    public static bool IsPlayable(this Card c)
    {
        return c.IsActive && IsPlayableBy(c.Type, c.Owner);
    }

    public static bool IsAnyFormPlayableByHero(this Card c)
        => c.IsActive && c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner) || IsPlayableBy(c.BasicType, c.Owner);
}
