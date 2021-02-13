
public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardTypeData c, Member member)
    {
        if (!member.IsConscious())
            return false;
        if (member.State[TemporalStatType.Disabled] > 0)
            return false;
        return member.CanAfford(c);
    }
    
    public static bool IsPlayableByHero(this Card c, BattleState b)
    {
        return c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner);
    }

    public static bool IsAnyFormPlayableByHero(this Card c)
        => c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner) || IsPlayableBy(c.BasicType, c.Owner);
}
