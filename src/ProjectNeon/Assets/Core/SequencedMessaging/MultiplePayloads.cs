public sealed class MultiplePayloads : IPayloadProvider
{
    private IPayloadProvider[] _payloadProviders;
    private int _index;

    public MultiplePayloads(IPayloadProvider[] payloadProviders) => _payloadProviders = payloadProviders;
        
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