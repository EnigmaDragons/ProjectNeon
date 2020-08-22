
public enum StatMathOperator
{
    Additive = 0,
    Multiplier = 1,
}

public static class StatMathOperatorExtensions
{
    public static string Describe(this StatMathOperator o)
    {
        if (o == StatMathOperator.Additive)
            return "+";
        if (o == StatMathOperator.Multiplier)
            return "x";
        return "??";
    }
}
