
public sealed class IndexSelector<T>
{
    private readonly T[] _items;
    private int _index;

    public IndexSelector(T[] items)
    {
        _items = items;
    }

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
}
