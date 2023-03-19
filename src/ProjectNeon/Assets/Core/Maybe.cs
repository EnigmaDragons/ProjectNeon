﻿using System;
using UnityEngine;

[Serializable]
public sealed class Maybe<T>
{
    [SerializeField] private T value;
    [SerializeField] private bool isPresent;

    public bool IsMissing => !isPresent;
    public bool IsPresent => isPresent && value != null; // Null check necessary since Unity objects can't be accessed when destroyed
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

    public void IfPresentAndMatches(Func<T, bool> condition, Action<T> action)
    {
        if (IsPresent && condition(value))
            action(value);
    }

    public void ExecuteIfPresentOrElse(Action<T> ifPresent, Action elseAction)
    {
        if (isPresent)
            ifPresent(Value);
        else
            elseAction();
    }

    public void Clear()
    {
        value = default(T);
        isPresent = false;
    }

    public Maybe<T> Set(T obj, bool newIsPresent)
    {
        value = obj;
        isPresent = newIsPresent;
        return this;
    }
    
    public bool IsPresentAnd(Func<T, bool> condition) => IsPresent && condition(value);
    public bool IsMissingOr(Func<T, bool> condition) => IsMissing || condition(value);

    public T OrDefault(T defaultValue) => IsPresent ? Value : defaultValue;
    public T OrDefault(Func<T> createDefault) => IsPresent ? Value : createDefault();
    
    public static Maybe<T> Missing() => new Maybe<T>(default(T), false);
    public static Maybe<T> Present(T val) => new Maybe<T>(val, true);
    
    public static implicit operator Maybe<T>(T obj) => new Maybe<T>(obj);
    public static implicit operator bool(Maybe<T> maybe) => maybe.IsPresent;

    public T2 Select<T2>(Func<T, T2> ifPresent, T2 def)
        => Select(ifPresent, () => def);
    public T2 Select<T2>(Func<T, T2> ifPresent, Func<T2> createDefault)
        => IsPresent 
            ? ifPresent(value) 
            : createDefault();
    
    public Maybe<T2> As<T2>() 
        => IsPresent && Value is T2 val
            ? new Maybe<T2>(val) 
            : Maybe<T2>.Missing();
    
    [Obsolete("Prefer Non-Allocating Version")]
    public Maybe<T2> Map<T2>(Func<T, T2> convert) 
        => IsPresent 
            ? new Maybe<T2>(convert(Value)) 
            : Maybe<T2>.Missing();
    
    public Maybe<T2> MapNonAlloc<T2>(Func<T, T2> convert, Maybe<T2> dest) 
        => IsPresent 
            ? dest.Set(convert(value), true) 
            : dest.Set(default(T2), false);

    public Maybe<T2> Chain<T2>(Func<T, Maybe<T2>> select)
        => IsPresent
            ? select(value)
            : Maybe<T2>.Missing();

    public bool Equals(Maybe<T> other)
    {
        if (other.IsMissing && IsMissing)
            return true;
        if (other.IsPresent && IsPresent)
            return other.Value.Equals(Value);
        return false;
    }

    public override string ToString() => Select(x => x.ToString(), "Missing " + typeof(T));
}
