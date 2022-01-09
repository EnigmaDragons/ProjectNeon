using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Notna")]
public class NotnaAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(x => true, CardTag.Attack)
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(x => x.Zones.HandZone.Cards.All(x => x.Mode != CardMode.Glitched), CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}