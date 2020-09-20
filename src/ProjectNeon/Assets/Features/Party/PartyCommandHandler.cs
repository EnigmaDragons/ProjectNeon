
using UnityEngine;

public class PartyCommandHandler : OnMessage<GivePartyCredits, GrantPartyLevelUp>
{
    [SerializeField] private PartyAdventureState party;

    protected override void Execute(GivePartyCredits msg) => party.UpdateCreditsBy(msg.Amount);
    protected override void Execute(GrantPartyLevelUp msg) => party.AwardLevelUpPoints(msg.NumPoints);
}
