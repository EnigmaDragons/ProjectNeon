
using UnityEngine;

public class PartyCommandHandler : OnMessage<GivePartyCredits, GivePartyXp>
{
    [SerializeField] private PartyAdventureState party;

    protected override void Execute(GivePartyCredits msg) => party.UpdateCreditsBy(msg.Amount);
    protected override void Execute(GivePartyXp msg) => party.AwardXp(msg.Xp);
}
