using System.Collections.Generic;
using System.Linq;

public sealed class MultiplePayloads : IPayloadProvider
{
    private readonly IPayloadProvider[] _payloadProviders;
    private int _index;

    public MultiplePayloads()
        : this("No Payload", new IPayloadProvider[0]) {}
    public MultiplePayloads(string name, object[] objects)
        : this(name, objects.Select(o => new SinglePayload(o))) {}
    public MultiplePayloads(string name, params IEnumerable<IPayloadProvider>[] payloadProviders) 
        : this(name, payloadProviders.SelectMany(p => p).ToArray()) {}
    public MultiplePayloads(string prefix, IEnumerable<IPayloadProvider> payloadProviders) 
        : this(prefix + " " + payloadProviders.FirstOrMaybe().Select(p => p.Name, "Nothing"), payloadProviders.ToArray()) {}

    public MultiplePayloads(string name, params IPayloadProvider[] payloadProviders)
    {
        Name = name;
        _payloadProviders = payloadProviders;
    }

    public string Name { get; }
    public int Count => _payloadProviders.Length;

    public bool IsFinished()
    {
        while (_index < _payloadProviders.Length && _payloadProviders[_index].IsFinished())
            _index++;
        return _index == _payloadProviders.Length;
    }

    public PayloadData GetNext()
    {
        while (_payloadProviders[_index].IsFinished())
            _index++;
        var payload = _payloadProviders[_index].GetNext();
        return payload;
    }
}
