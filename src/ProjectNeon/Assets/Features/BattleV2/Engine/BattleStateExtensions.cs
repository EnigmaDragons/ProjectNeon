
using System.Linq;

public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardTypeData c, Member member, PartyAdventureState partyState)
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
        if (member.State.IsPrevented(c.Tags))
            return false;
        return member.CanAfford(c, partyState);
    }
    
    public static bool IsPlayable(this Card c, PartyAdventureState partyState)
    {
        return c.IsActive && IsPlayableBy(c.Type, c.Owner, partyState);
    }

    public static bool IsAnyFormPlayableByHero(this Card c, PartyAdventureState partyState)
        => c.IsActive && c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner, partyState) || IsPlayableBy(c.Owner.BasicCard.Value, c.Owner, partyState);

    public static bool HasAnyValidTargets(this CardType c, Member m, BattleState state)
        => c.ActionSequences.All(action
            => action.Scope == Scope.All
            || action.Scope == Scope.AllExceptSelf
            || action.Group == Group.Self
            || action.Group == Group.Ally
            || action.Group == Group.All
            || state.MembersWithoutIds.Any(x => x.TeamType != m.TeamType && !x.IsStealthed() && x.IsConscious()));
}
