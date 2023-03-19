using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoEnemiesWithEnoughMarks")]
public class NoEnemiesWithEnoughMarks : StaticCardCondition
{
    [SerializeField] private int requiredMarks;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoEnemy(x => x.State[TemporalStatType.Marked] >= requiredMarks);
    
    public override string Description => "Thoughts/Condition022".ToLocalized();
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition022" };
}