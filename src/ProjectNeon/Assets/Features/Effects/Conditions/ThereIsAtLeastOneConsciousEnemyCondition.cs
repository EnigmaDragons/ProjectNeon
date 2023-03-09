using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/ThereIsAtLeastOneConsciousEnemyCondition")]
public class ThereIsAtLeastOneConsciousEnemyCondition : StaticEffectCondition
{
    [SerializeField] private BattleState state;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var conscious = state.Members.Where(m => m.Value.IsConscious() && m.Value.TeamType != ctx.Source.TeamType);
        return conscious.Any()
            ? Maybe<string>.Missing()
            : new Maybe<string>($"all enemies are not conscious");
    }
}