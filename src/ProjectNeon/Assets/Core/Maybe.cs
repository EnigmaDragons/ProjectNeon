using System;

public sealed class Maybe<T> where T : class
{
    private readonly T _value;

    public bool IsPresent { get; }

    public T Value
    {
        get
        {
            if (!IsPresent)
                throw new InvalidOperationException($"Optional {typeof(T).Name} has no value.");
            return _value;
        }
    }

    public Maybe() { }

    public Maybe(T value)
    {
        _value = value;
        IsPresent = value != null;
    }

    public bool IsTrue(Predicate<T> condition)
    {
        return IsPresent && condition(_value);
    }

    public bool IsFalse(Predicate<T> condition)
    {
        return IsPresent && !condition(_value);
    }
}
