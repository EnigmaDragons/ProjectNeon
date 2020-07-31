
public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardType c, Member member)
    {
        if (!member.IsConscious())
            return false;
        return member.CanAfford(c);
    }
    
    public static bool IsPlayableByHero(this Card c, BattleState b)
    {
        return c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner);
    }
}
