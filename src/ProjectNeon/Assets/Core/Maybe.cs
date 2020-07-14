using System;
using UnityEngine;

[Serializable]
public sealed class Maybe<T> where T : class
{
    [SerializeField] private T value;
    [SerializeField] private bool isPresent;

    public bool IsMissing => !isPresent;
    public bool IsPresent => isPresent;
    public T Value => value;

    public Maybe() { }

    public Maybe(T obj)
    {
        value = obj;
        isPresent = obj != null;
    }
    
    public void IfPresent(Action<T> action)
    {
        if (IsPresent)
            action(value);
    }

    public bool IsPresentAnd(Func<T, bool> condition) => IsPresent && condition(value);

    public T OrDefault(Func<T> createDefault) => IsPresent ? Value : createDefault();
    
    public static Maybe<T> Missing() => new Maybe<T>();
    
    public static implicit operator Maybe<T>(T obj) => new Maybe<T>(obj);
}
