
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/CanChain")]
public class CanChain : StaticCardCondition
{
    public override bool ConditionMet(CardConditionContext ctx)
    {
        throw new NotImplementedException();
        // var history = 
        // => ctx.BattleState.NumberOfCardPlaysRemainingThisTurn == 1 && ctx
    }
}