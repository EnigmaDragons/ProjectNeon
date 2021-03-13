using UnityEngine;

[CreateAssetMenu(menuName = "AI/Electro Drone")]
public class ElectroDroneAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .IfTrueDontPlayType(ctx => ctx.Member.CurrentShield() <= 26, CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedTargetsPlayedCard();
    }
}