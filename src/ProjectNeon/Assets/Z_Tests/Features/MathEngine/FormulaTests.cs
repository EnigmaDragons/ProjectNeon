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

    [Test] public void Formula_RawStatValue() 
        => AssertResultsIs(2, "Attack", TestMembers.Create(s => s.With(StatType.Attack, 2)));

    [Test] public void Formula_StatValue_Multiply()
        => AssertResultsIs(3, "Attack * 1.5", TestMembers.Create(s => s.With(StatType.Attack, 2)));
    
    [Test] public void Formula_RawTemporalStatValue() 
        => AssertResultsIs(1, "Blind", TestMembers.Create(s => s.With(TemporalStatType.Blind, 1)));

    [Test] public void Formula_MultiplyByStatAndAdd() 
        => AssertResultsIs(7, "4 + Attack * 1.5", TestMembers.Create(s => s.With(StatType.Attack, 2)));

    private void AssertResultsIs(int val, string exp) 
        => AssertResultsIs(val, exp, new FormulaContext(TestMembers.Any()));
    
    private void AssertResultsIs(int val, string exp, Member m) 
        => AssertResultsIs(val, exp, new FormulaContext(m));
    
    private void AssertResultsIs(int val, string exp, FormulaContext ctx) 
        => Assert.AreEqual(val, Formula.Evaluate(ctx, exp));
}
