﻿using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/Unprofitable")]
public class Unprofitable : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.BattleState.Party.Credits <= ctx.BattleState.CreditsAtStartOfBattle;
    
    public override string Description => $"We have not made money this fight";
}