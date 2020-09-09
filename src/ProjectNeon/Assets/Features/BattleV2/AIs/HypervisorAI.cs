using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hypervisor")]
public class HypervisorAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(c => c.Allies.None(x => x.CurrentHp() + x.CurrentShield() > x.MaxHp() * 0.5f), CardTag.BuffAttack, CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .IfTrueDontPlayType(c => c.Allies.Length > 1, CardTag.Attack)
            .IfTrueDontPlayType(c => c.Allies.Length < 3, CardTag.Group, CardTag.BuffAttack)
            .WithSelectedTargetsPlayedCard();
    }
}
