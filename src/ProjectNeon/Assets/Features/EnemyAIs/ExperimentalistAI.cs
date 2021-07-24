using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Experimentalist")]
public class ExperimentalistAI : StatelessTurnAI
{
    protected override IPlayedCard Select(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var card = new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .IfTrueDontPlayType(ctx => ctx.Allies.All(x => x.CurrentShield() > 0), CardTag.Ultimate)
            .WithSelectedUltimateIfAvailable()
            .IfTrueDontPlayType(ctx => ctx.Allies.All(x => x.Armor() >= 10 || x.CurrentHp() <= 3), CardTag.Defense)
            .WithFinalizedCardSelection(type => type.Is(CardTag.Buff) ? 1 : 0);
        if (card.SelectedCard.Value.Is(CardTag.Defense))
            return new PlayedCardV2(
                battleState.Members[memberId], 
                new Target[] { battleState.EnemyMembers.Where(x => x.Armor() < 10 && x.CurrentHp() > 3).Select(x => new Single(x)).MostVulnerable() }, 
                card.SelectedCard.Value.CreateInstance(battleState.GetNextCardId(), battleState.Members[memberId]));
        return card.WithSelectedTargetsPlayedCard();
    }
}