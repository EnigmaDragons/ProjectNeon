using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MedicAI")]
public sealed class MedicAI : TurnAI
{
    private static readonly DictionaryWithDefault<CardTag, int> CardTypePriority = new DictionaryWithDefault<CardTag, int>(99)
    {
        { CardTag.Healing, 1 },
        { CardTag.Defense, 2 },
    };
    
    public override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var me = battleState.Members[memberId];
        var playableCards = battleState.GetPlayableCards(memberId);
        var allies = me.TeamType == TeamType.Enemies 
            ? battleState.Enemies.Where(m => m.IsConscious()) 
            : battleState.Heroes.Where(m => m.IsConscious());
        
        var ctx = new CardSelectionContext(me, battleState, strategy)
            .WithOptions(playableCards)
            .WithSelectedDesignatedAttackerCardIfApplicable();
        // TODO: Dealing killing blow if possible with an attack card
        
        IEnumerable<CardTypeData> cardOptions = playableCards;
        // Don't play a heal if all allies are very healthy
        if (allies.All(a => a.CurrentHp() >= a.MaxHp() * 0.9))
            cardOptions = cardOptions.Where(c => !c.Is(CardTag.Healing));

        // Don't play a shield if all allies are already shielded
        if (allies.All(a => a.RemainingShieldCapacity() > a.MaxShield() * 0.7))
            cardOptions = cardOptions.Where(x => !x.Is(CardTag.Defense, CardTag.Shield));

        return ctx.WithOptions(cardOptions)
            .WithFinalizedCardSelection(c => CardTypePriority[c.Tags.First()])
            .WithSelectedTargetsPlayedCard();
    }
}
