using System;

public static class ConditionExtensions
{
    public static void If(this bool evaluatedCondition, Action onTrue)
    {
        if (evaluatedCondition)
            onTrue();
    }

    public static void IfNull(this object o, Action onTrue) => (o == null).If(onTrue);
}
