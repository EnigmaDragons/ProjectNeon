
using System;

public sealed class IndexSelector<T>
{
    private readonly T[] _items;
    private int _index;

    public IndexSelector(T[] items, int index = 0)
    {
        _index = Math.Min(index, items.Length);
        _items = items;
    }

    public int Count => _items.Length;
    public int Index => _index;
    public T Current => _items[_index];

    public T MoveNext()
    {
        _index = (_index + 1) % _items.Length;
        return Current;
    }

    public T MovePrevious()
    {
        var next = (_index - 1) % _items.Length;
        if (next < 0)
            next = _items.Length - 1;
        _index = next;
        return Current;
    }

    public T MovePreviousWithoutLooping()
    {
        if (_index > 0)
            _index--;
        return Current;
    }
    
    public T MoveNextWithoutLooping()
    {
        if (_index < _items.Length - 1)
            _index++;
        return Current;
    }
}
