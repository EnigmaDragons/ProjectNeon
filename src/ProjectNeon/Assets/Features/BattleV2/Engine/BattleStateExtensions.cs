
using System.Linq;

public static class BattleStateExtensions
{
    public static bool IsPlayableBy(this CardTypeData c, Member member, PartyAdventureState partyState, int numberOfCardPlaysRemainingThisTurn)
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
        if (numberOfCardPlaysRemainingThisTurn <= 0 && c.Speed == CardSpeed.Standard)
            return false;
        return member.CanAfford(c, partyState);
    }
    
    public static bool IsPlayable(this Card c, PartyAdventureState partyState, int numberOfCardPlaysRemainingThisTurn)
    {
        return c.IsActive && IsPlayableBy(c.Type, c.Owner, partyState, numberOfCardPlaysRemainingThisTurn);
    }

    public static bool IsAnyFormPlayableByHero(this Card c, PartyAdventureState partyState, int numberOfCardPlaysRemainingThisTurn)
        => c.IsActive && c.Owner.TeamType == TeamType.Party && IsPlayableBy(c.Type, c.Owner, partyState, numberOfCardPlaysRemainingThisTurn) || IsPlayableBy(c.Owner.BasicCard.Value, c.Owner, partyState, numberOfCardPlaysRemainingThisTurn);

    public static bool HasAnyValidTargets(this CardType c, Member m, BattleState state)
        => c.ActionSequences.All(action
            => action.Scope == Scope.All
            || action.Scope == Scope.AllExceptSelf
            || action.Group == Group.Self
            || action.Group == Group.Ally
            || action.Group == Group.All
            || state.MembersWithoutIds.Any(x => x.TeamType != m.TeamType && !x.IsStealthed() && x.IsConscious()));
}
