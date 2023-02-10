using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AllEnemiesHaveMarks")]
public class AllEnemiesHaveMarks : StaticCardCondition
{
    [SerializeField] private int marks;
    
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AllEnemies(x => x.State[TemporalStatType.Marked] >= marks);
    
    public override string Description => $"All enemies have at least {marks} marks";
}