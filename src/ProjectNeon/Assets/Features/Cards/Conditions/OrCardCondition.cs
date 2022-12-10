using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OrCardCondition")]
public class OrCardCondition : StaticCardCondition
{
    [SerializeField] private StaticCardCondition[] conditions;

    public override bool ConditionMet(CardConditionContext ctx)
        => conditions.Any(x => x.ConditionMet(ctx));
    
    public override string Description => $"One of multiple or conditions was met";
}
