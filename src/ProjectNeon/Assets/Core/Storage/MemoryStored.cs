using System;

public class MemoryStored<T> : Stored<T>
{
    private T _value;

    public MemoryStored(T initialValue) => _value = initialValue;

    public T Get() => _value;

    public void Write(Func<T, T> transform) => _value = transform(_value);
}
