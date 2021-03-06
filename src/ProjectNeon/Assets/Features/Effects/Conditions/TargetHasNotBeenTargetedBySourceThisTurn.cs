﻿using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "EffectConditions/TargetHasNotBeenTargetedBySourceThisTurn")]
public class TargetHasNotBeenTargetedBySourceThisTurn : StaticEffectCondition
{
    public override Maybe<string> GetShouldNotApplyReason(EffectContext ctx)
    {
        return ctx.Target.Members.All(
            member => ctx.CardsPlayedThisTurn.Take(ctx.CardsPlayedThisTurn.Length - 1).All(
                card => card.Member.Id != ctx.Source.Id
                        || !card.Targets.Any(
                            target => target.Members.Any(
                                x => x.Id == member.Id))))
            ? Maybe<string>.Missing()
            : new Maybe<string>("One of the target members has been targeted by the source this turn");
    }
}