
public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardType c, Member member)
    {
        if (!member.IsConscious())
            return false;
        return member.CanAfford(c);
    }
    
    public static bool IsPlayableByHero(this CardType c, BattleState b)
    {
        if (c.LimitedToClass.IsMissing)
            return false;
        
        var member = b.GetMemberByClass(c.LimitedToClass.Value);
        if (!member.IsConscious())
            return false;

        return member.CanAfford(c);
    }

    public static bool HeroIsConscious(this Card c, BattleState b)
    {
        return b.GetMemberByClass(c.LimitedToClass.Value).IsConscious();
    }
}
