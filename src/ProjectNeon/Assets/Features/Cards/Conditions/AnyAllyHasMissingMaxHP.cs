using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/AnyAllyHasMissingMaxHP")]
public class AnyAllyHasMissingMaxHP : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.AnyAlly(x => x.MaxHp() < x.State.BaseStats.MaxHp());
}