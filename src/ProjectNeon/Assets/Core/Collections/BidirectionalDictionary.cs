using System.Collections;
using System.Collections.Generic;

public sealed class BidirectionalDictionary<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
{
    private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
    private readonly Dictionary<T2, T1> _backward = new Dictionary<T2, T1>();

    public BidirectionalDictionary() { }
    public BidirectionalDictionary(IEnumerable<KeyValuePair<T1, T2>> items)
    {
        items.ForEach(x => Add(x.Key, x.Value));
    }

    public void Add(T1 item1, T2 item2)
    {
        _forward.Add(item1, item2);
        _backward.Add(item2, item1);
    }
    
    public IEnumerable<T1> Keys => _forward.Keys;
    public IEnumerable<T2> Values => _forward.Values;
    public T1 this[T2 value] => _backward[value];
    public T2 this[T1 value] => _forward[value];
    public bool TryGetValue(T1 item1, out T2 item2) => _forward.TryGetValue(item1, out item2);
    public bool TryGetKey(T2 item2, out T1 item1) => _backward.TryGetValue(item2, out item1);
    public Dictionary<T1, T2>.Enumerator GetEnumerator() => _forward.GetEnumerator();
    IEnumerator<KeyValuePair<T1, T2>> IEnumerable<KeyValuePair<T1, T2>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
