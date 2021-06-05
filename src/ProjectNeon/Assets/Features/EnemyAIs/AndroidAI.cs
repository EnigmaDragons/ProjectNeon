using UnityEngine;

[CreateAssetMenu(menuName = "AI/Android")]
public class AndroidAI : TurnAI
{
    public override IPlayedCard InnerPlay(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return Anticipate(memberId, battleState, strategy);
    }

    public override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
    {
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithFinalizedCardSelection(x => x.Tags.Contains(CardTag.Glitch) ? 1 : 2)
            .WithSelectedTargetsPlayedCard();
    }
}