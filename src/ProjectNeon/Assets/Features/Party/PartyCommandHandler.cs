
using UnityEngine;

public class PartyCommandHandler : OnMessage<GivePartyCredits>
{
    [SerializeField] private PartyAdventureState party;

    protected override void Execute(GivePartyCredits msg) => party.UpdateCreditsBy(msg.Amount);
}
