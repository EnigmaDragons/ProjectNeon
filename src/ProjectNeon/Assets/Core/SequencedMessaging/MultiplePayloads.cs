using System.Collections.Generic;
using System.Linq;

public sealed class MultiplePayloads : IPayloadProvider
{
    private readonly IPayloadProvider[] _payloadProviders;
    private int _index;

    public MultiplePayloads() 
        : this(new IPayloadProvider[0]) {}
    public MultiplePayloads(params IEnumerable<IPayloadProvider>[] payloadProviders) 
        : this(payloadProviders.SelectMany(p => p).ToArray()) {}
    public MultiplePayloads(IEnumerable<IPayloadProvider> payloadProviders) 
        : this(payloadProviders.ToArray()) {}
    public MultiplePayloads(params IPayloadProvider[] payloadProviders) => _payloadProviders = payloadProviders; 
    
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
