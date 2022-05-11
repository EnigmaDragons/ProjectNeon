using UnityEngine;

public class CardPlayedVisualController : OnMessage<EnemyCardPlayed, ReactionCardPlayed>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject cardPlayedPrototype;
    [SerializeField] private Vector3 offset;
    
    protected override void Execute(EnemyCardPlayed msg)
    {
        var maybeCenterpoint = state.GetMaybeCenterPoint(msg.PlayedCard.MemberId());
        maybeCenterpoint.IfPresent(c => Instantiate(cardPlayedPrototype, c.position + offset, Quaternion.identity, transform));
    }

    protected override void Execute(ReactionCardPlayed msg)
    {
        var maybeCenterpoint = state.GetMaybeCenterPoint(msg.PlayedCard.MemberId());
        maybeCenterpoint.IfPresent(c => Instantiate(cardPlayedPrototype, c.position + offset, Quaternion.identity, transform));
    }
}
