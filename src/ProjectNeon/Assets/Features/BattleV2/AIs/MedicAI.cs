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
        // TODO: Dealing killing blow if possible with an attack card
        return new CardSelectionContext(memberId, battleState, strategy)
            .WithSelectedDesignatedAttackerCardIfApplicable()
            .WithCommonSenseSelections()
            .WithFinalizedCardSelection(c => CardTypePriority[c.Tags.First()])
            .WithSelectedTargetsPlayedCard();
    }
}
