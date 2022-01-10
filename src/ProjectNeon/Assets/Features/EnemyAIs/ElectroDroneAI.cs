using UnityEngine;

[CreateAssetMenu(menuName = "AI/Electro Drone")]
public class ElectroDroneAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(ctx => ctx.Member.CurrentShield() <= 32, CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedTargetsPlayedCard();
    }
}