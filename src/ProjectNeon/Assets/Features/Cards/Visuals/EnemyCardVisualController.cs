using UnityEngine;

public class EnemyCardVisualController : OnMessage<EnemyCardPlayed>
{
    [SerializeField] private BattleState state;
    [SerializeField] private GameObject cardPlayedPrototype;
    [SerializeField] private Vector3 offset;
    
    protected override void Execute(EnemyCardPlayed msg)
    {
        var maybeCenterpoint = state.GetMaybeCenterPoint(msg.PlayedCard.MemberId());
        maybeCenterpoint.IfPresent(c => Instantiate(cardPlayedPrototype, c.position + offset, Quaternion.identity, transform));
    }
}
