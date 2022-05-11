using UnityEngine;

public class HpDamageBloodEffect : OnMessage<MemberStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private float fxDelay;
    
    protected override void Execute(MemberStateChanged msg)
    {
        if (msg.State.MissingHp() > msg.BeforeState.MissingHp)
            state.GetMaybeCenterPoint(msg.MemberId())
                .IfPresent(cp => this.ExecuteAfterDelay(
                    () => Message.Publish(new PlayRawBattleEffect("HpDamage", cp.position)), fxDelay));
    }
}
