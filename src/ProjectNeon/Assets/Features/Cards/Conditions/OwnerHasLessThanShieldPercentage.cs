﻿using UnityEngine;

[CreateAssetMenu(menuName = "CardConditions/OwnerHasLassThanShieldPercentage")]
public class OwnerHasLessThanShieldPercentage : StaticCardCondition
{
    [SerializeField] private float percent;

    public override bool ConditionMet(CardConditionContext ctx)
        => ctx.Card.Owner.CurrentShield() < ctx.Card.Owner.MaxShield() * percent;
    
    public override string Description => "Thoughts/Condition031".ToLocalized().SafeFormatWithDefault("I have less than {0}% of my maximum shields", percent * 100);
    public override string[] GetLocalizeTerms() => new [] { "Thoughts/Condition031" };
}