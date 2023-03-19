using System.Linq;
using UnityEngine;

public class OnHeroUnconsciousClearOutTheirCardsFromNonHand : OnMessage<MemberUnconscious>
{
    [SerializeField] private CardPlayZones cardPlayZones;
    
    protected override void Execute(MemberUnconscious msg)
    {
        var memberCards = cardPlayZones.DrawZone.DrawWhere(c => c.Owner.Id == msg.Member.Id)
            .Concat(cardPlayZones.DiscardZone.DrawWhere(c => c.Owner.Id == msg.Member.Id));

        memberCards.ForEach(c => cardPlayZones.VoidZone.PutOnBottom(c));
    }
}
