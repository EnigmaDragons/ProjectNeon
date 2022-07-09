using UnityEngine;

public class HpDamageBloodEffect : OnMessage<MemberStateChanged>
{
    [SerializeField] private BattleState state;
    [SerializeField] private float fxDelay;

    private const string OrganicDamageEffect = "HpDamage";
    private const string MetallicDamageEffect = "HpDamageMetallic";
    
    protected override void Execute(MemberStateChanged msg)
    {
        var damageEffectType = state.GetMaterialType(msg.MemberId()) == MemberMaterialType.Organic 
            ? OrganicDamageEffect 
            : MetallicDamageEffect;
        
        if (msg.State.MissingHp() > msg.BeforeState.MissingHp)
            state.GetMaybeCenterPoint(msg.MemberId())
                .IfPresent(cp => this.ExecuteAfterDelay(
                    () => Message.Publish(new PlayRawBattleEffect(damageEffectType, cp.position)), fxDelay));
    }
}
