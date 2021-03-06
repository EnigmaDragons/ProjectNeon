using NUnit.Framework;

public class FormulaTests
{
    [Test] public void Formula_ConstantValue() 
        => AssertResultsIs(3, "3");

    [Test] public void Formula_Add_ConstantValues() 
        => AssertResultsIs(6, "3 + 3");

    [Test] public void Formula_Multiply_ConstantValues() 
        => AssertResultsIs(9, "3 * 3");
    
    [Test] public void Formula_Subtract_ConstantValues() 
        => AssertResultsIs(1, "4 - 3");
    
    [Test] public void Formula_Divide_ConstantValues() 
        => AssertResultsIs(2, "4 / 2");
    
    [Test] public void Formula_Parentheses_ConstantValues() 
        => AssertResultsIs(10, "(4 + 1) * 2");

    [Test] public void Formula_RawStatValue() 
        => AssertResultsIs(2, "Attack", TestMembers.Create(s => s.With(StatType.Attack, 2)));

    [Test] public void Formula_StatValue_Multiply()
        => AssertResultsIs(3, "Attack * 1.5", TestMembers.Create(s => s.With(StatType.Attack, 2)));
    
    [Test] public void Formula_RawTemporalStatValue() 
        => AssertResultsIs(1, "Blind", TestMembers.Create(s => s.With(TemporalStatType.Blind, 1)));

    [Test] public void Formula_MultiplyByStatAndAdd() 
        => AssertResultsIs(7, "4 + Attack * 1.5", TestMembers.Create(s => s.With(StatType.Attack, 2)));

    [Test] public void Formula_Leadership() 
        => AssertResultsIs(1.8f, "0.18 * Leadership", TestMembers.Create(s => s.With(StatType.Leadership, 10)));
    
    [Test] public void Formula_Economy() 
        => AssertResultsIs(1.8f, "0.18 * Economy", TestMembers.Create(s => s.With(StatType.Economy, 10)));
    
    [Test] public void Formula_PercentOfHp() 
        => AssertResultsIs(10, "0.5 * HP", TestMembers.Create(s => s.With(StatType.MaxHP, 20)));
    
    [Test] public void Formula_TargetStat() 
        => AssertResultsIs(20, "Target[HP]", new FormulaContext(TestMembers.Any().GetSnapshot().State, TestMembers.Create(s => s.With(StatType.MaxHP, 20)), ResourceQuantity.None));
    
    [Test] public void Formula_ResourceAmount()
        => AssertResultsIs(2, "Flames", TestMembers.Create(s => s.With(new InMemoryResourceType("Flames") { StartingAmount = 2, MaxAmount = 99})));

    [Test] public void Formula_XAmountPaid()
        => AssertResultsIs(1, "X", new FormulaContext(TestMembers.Any().GetSnapshot().State, TestMembers.Any(), new ResourceQuantity { Amount = 1 }));
    
    [Test] public void Formula_DoubleDamageIfStunned()
        => AssertResultsIs(2, "1 {Target[CardStun] > 0:* 2}", 
            new FormulaContext(TestMembers.Any().GetSnapshot().State, TestMembers.Create(x => x.With(TemporalStatType.CardStun, 1)), ResourceQuantity.None));
    
    [Test] public void Formula_PrimaryResource()
        => AssertResultsIs(1, "PrimaryResource", 
            new FormulaContext(TestMembers.With(new InMemoryResourceType("prime") { MaxAmount = 1, StartingAmount = 1 }).GetSnapshot().State, Maybe<MemberState>.Missing(), ResourceQuantity.None));
    
    [Test] public void Formula_TargetPrimaryResource()
        => AssertResultsIs(1, "Target[PrimaryResource]", 
            new FormulaContext(TestMembers.Any().GetSnapshot().State, TestMembers.With(new InMemoryResourceType("prime") { MaxAmount = 1, StartingAmount = 1 }), ResourceQuantity.None));
    
    private void AssertResultsIs(float val, string exp) 
        => AssertResultsIs(val, exp, new FormulaContext(TestMembers.Any().GetSnapshot().State, TestMembers.Any().State, ResourceQuantity.None));
    
    private void AssertResultsIs(float val, string exp, Member m) 
        => AssertResultsIs(val, exp, new FormulaContext(m.GetSnapshot().State, TestMembers.Any().State, ResourceQuantity.None));
    
    private void AssertResultsIs(float val, string exp, FormulaContext ctx) 
        => Assert.AreEqual(val, Formula.Evaluate(ctx, exp));
}
