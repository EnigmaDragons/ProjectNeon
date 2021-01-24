using System;
using UnityEngine;

[Serializable]
public sealed class Maybe<T>
{
    [SerializeField] private T value;
    [SerializeField] private bool isPresent;

    public bool IsMissing => !isPresent;
    public bool IsPresent => isPresent;
    public T Value => value;

    public Maybe(T obj)
        : this(obj, obj != null && !obj.Equals(default(T))) {}
    
    public Maybe(T obj, bool isPresent)
    {
        value = obj;
        this.isPresent = isPresent;
    }
    
    public void IfPresent(Action<T> action)
    {
        if (IsPresent)
            action(value);
    }

    public bool IsPresentAnd(Func<T, bool> condition) => IsPresent && condition(value);
    public bool IsMissingOr(Func<T, bool> condition) => IsMissing || condition(value);

    public T OrDefault(T defaultValue) => IsPresent ? Value : defaultValue;
    public T OrDefault(Func<T> createDefault) => IsPresent ? Value : createDefault();
    
    public static Maybe<T> Missing() => new Maybe<T>(default(T), false);
    
    public static implicit operator Maybe<T>(T obj) => new Maybe<T>(obj);

    public T2 Select<T2>(Func<T, T2> ifPresent, T2 def)
        => Select(ifPresent, () => def);
    public T2 Select<T2>(Func<T, T2> ifPresent, Func<T2> createDefault)
        => IsPresent 
            ? ifPresent(value) 
            : createDefault();
}
