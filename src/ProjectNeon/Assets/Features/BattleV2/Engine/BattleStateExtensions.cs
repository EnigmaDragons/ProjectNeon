
public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardTypeData c, Member member)
    {
        if (!member.IsConscious())
            return false;
        if (member.State[TemporalStatType.TurnStun] > 0)
            return false;
        return member.CanAfford(c.Cost);
    }
    
    public static bool IsPlayableByHero(this Card c, BattleState b)
    {
        return c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner);
    }
}
