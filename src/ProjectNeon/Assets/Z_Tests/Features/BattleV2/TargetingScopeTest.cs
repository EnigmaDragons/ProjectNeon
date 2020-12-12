
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;


[TestFixture]
public sealed class TargetingScopeTest
{
    [Test]
    public void AllExceptSelfTest()
    {
        
        Member self = TestMembers.Any();
        Member other = TestMembers.Any();
        Member other2 = TestMembers.Any();
        Member[] allMembers = {self, other, other2};
        Member[] justOther = {other, other2};


        var targets = BattleStateTargetingExtensions.GetPossibleConsciousTargets(allMembers, self, Group.Ally, Scope.AllExceptSelf);
        
        Assert.AreEqual(justOther, targets[0].Members);
        
    }
    
    
}