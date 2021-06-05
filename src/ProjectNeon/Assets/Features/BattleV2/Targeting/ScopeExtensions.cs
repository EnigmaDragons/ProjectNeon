public static class ScopeExtensions
{
    public static Scope Inverted(this Scope scope)
    {
        if (scope == Scope.AllExceptSelf)
            return Scope.All;
        if (scope == Scope.OneExceptSelf)
            return Scope.One;
        return scope;
    }
}