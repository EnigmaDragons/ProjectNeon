using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Experimentalist")]
public class ExperimentalistAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(_ => true, CardTag.Attack)
            .IfTrueDontPlayType(ctx => ctx.Allies.All(x => x.CurrentShield() > 0), CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .IfTrueDontPlayType(ctx => strategy.DesignatedAttacker.IsConfused(), CardTag.Confusion)
            .WithFinalizedCardSelection();
        if (card.SelectedCard.Value.Is(CardTag.Confusion))
            return new PlayedCardV2(
                battleState.Members[memberId], 
                new Target[] { new Single(strategy.DesignatedAttacker) }, 
                card.SelectedCard.Value.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        return card.WithSelectedTargetsPlayedCard();
    }
}