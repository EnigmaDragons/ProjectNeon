using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/NoAllyHasMissingMaxHP")]
public class NoAllyHasMissingMaxHP : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.NoAlly(x => x.MaxHp() < x.State.BaseStats.MaxHp());
    
    public override string Description => "No ally is missing maximum health";
}