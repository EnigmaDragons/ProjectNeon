using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesHaveMarks")]
public class AllEnemiesHaveMarks : StaticCardCondition
{
    [SerializeField] private int marks;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State[TemporalStatType.Marked] >= marks);
    
    public override string Description => "Thoughts/Condition003".ToLocalized().SafeFormatWithDefault("All enemies have at least {0} marks", marks);
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition003" };
}