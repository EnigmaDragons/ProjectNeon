using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Controller")]
public class ControllerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {        
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithCommonSenseSelections()
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithSelectedUltimateIfAvailable()
            .WithFinalizedCardSelection()
            .WithSelectedTargetsPlayedCard();
    }
}
