using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesHaveAnyTemporalStatType")]
public class NoEnemiesHaveAnyTemporalStatType : StaticCardCondition
{
    [SerializeField] private TemporalStatType statType;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State[statType] > 0);
    
    public override string Description => string.Format("Thoughts/Condition019".ToLocalized(), $"Stats/Stat-{statType}".ToLocalized());
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition019" };
}
