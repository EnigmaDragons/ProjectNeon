﻿using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/EnemiesBloodied")]
public class EnemiesBloodied : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.EnemyMembers.Any(x => x.IsBloodied());
}