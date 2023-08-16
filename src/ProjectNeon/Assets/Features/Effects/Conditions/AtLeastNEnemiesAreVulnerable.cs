using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/AtLeastNEnemiesAreVulnerable")]
public class AtLeastNEnemiesAreVulnerable : StaticEffectCondition
{
    [SerializeField] private BattleState state;
    [SerializeField] private int minimumVulnerableCount = 2;
    
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        var vulnerableEnemies = state.Members.Where(m => m.Value.IsVulnerable() && m.Value.TeamType != ctx.Source.TeamType);
        return vulnerableEnemies.Count() >= minimumVulnerableCount
            ? Maybe<string>.Missing()
            : new Maybe<string>($"There are not at least {minimumVulnerableCount} vulnerable enemies");
    }
}
