using UnityEngine;

[CreateAssetMenu(menuName = "AI/Hacker")]
public class HackerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedUltimateIfAvailable()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .IfTrueDontPlayType(_ => true, CardTag.Attack)
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}