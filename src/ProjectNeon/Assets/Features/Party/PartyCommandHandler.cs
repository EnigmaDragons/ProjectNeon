
using UnityEngine;

public class PartyCommandHandler : OnMessage<GivePartyCredits, GivePartyXp, AddHeroToPartyRequested, RemoveHeroFromPartyRequested>
{
    [SerializeField] private PartyAdventureState party;

    protected override void Execute(GivePartyCredits msg) => party.UpdateCreditsBy(msg.Amount);
    protected override void Execute(GivePartyXp msg) => party.AwardXp(msg.Xp);
    protected override void Execute(AddHeroToPartyRequested msg) => party.WithAddedHero(msg.Hero);
    protected override void Execute(RemoveHeroFromPartyRequested msg) => party.WithRemovedHero(msg.Hero);
}
