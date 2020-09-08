using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/TankAI")]
public sealed class TankAI : TurnAI
{
    private static readonly DictionaryWithDefault<CardTag, int> CardTypePriority = new DictionaryWithDefault<CardTag, int>(99)
    {
        { CardTag.Defense, 1 },
        { CardTag.Attack, 2}
    };
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies 
            ? battleState.Enemies.Where(m => m.IsConscious()) 
            : battleState.Heroes.Where(m => m.IsConscious());
        
        // TODO: Dealing killing blow if possible with an attack card

        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable();
        
        IEnumerable<CardTypeData> cardOptions = playableCards;
        // Don't play a shield if all allies are already shielded
        if (allies.All(a => a.RemainingShieldCapacity() > a.MaxShield() * 0.7))
            cardOptions = cardOptions.Where(x => !x.Is(CardTag.Defense, CardTag.Shield));

        return ctx.WithOptions(cardOptions)
            .WithFinalizedCardSelection(c => CardTypePriority[c.Tags.First()])
            .WithSelectedTargetsPlayedCard();
    }
}

