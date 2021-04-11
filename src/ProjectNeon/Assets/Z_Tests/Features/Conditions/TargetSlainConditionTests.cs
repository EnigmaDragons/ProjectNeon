using System;
using NUnit.Framework;

public sealed class TargetSlainConditionTests
{
    [Test]
    public void TargetSlainCondition_TargetNotUnconscious_NoEffect()
    {
        var condition = new TargetSlainCondition(TestEffects.EmptyCardActionsData());

        var payload = condition.Resolve(new CardActionContext(TestMembers.Any(), new Single(TestMembers.Any()),
            Group.Opponent, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
        
        Assert.IsTrue(payload.IsFinished());
    }
    
    [Test]
    public void TargetSlainCondition_TargetUnconscious_HasEffect()
    {
        var condition = new TargetSlainCondition(TestEffects.EmptyCardActionsData());

        var payload = condition.Resolve(new CardActionContext(TestMembers.Any(), new Single(TestMembers.Any().Apply(m => m.TakeDamage(20))), 
            Group.Opponent, Scope.One, ResourceQuantity.None, new BattleStateSnapshot(), Maybe<Card>.Missing()));
        
        Assert.IsFalse(payload.IsFinished());
    }
}
