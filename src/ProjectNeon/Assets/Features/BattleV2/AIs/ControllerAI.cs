using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Controller")]
public class ControllerAI : TurnAI
{
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {        
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var enemies = battleState.GetConsciousEnemies(me);

        IEnumerable<CardTypeData> cardOptions = playableCards;

        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithSelectedUltimateIfAvailable();
        
        // Don't Hit Enemy Shields if the play isn't very effective
        if (enemies.Sum(e => e.CurrentShield()) < 15)
            cardOptions = cardOptions.Where(c => !c.Tags.Contains(CardTag.Shield));

        ctx = ctx.WithOptions(cardOptions)
            .WithFinalizedCardSelection();

        return ctx.WithSelectedTargetsPlayedCard();
    }
}
