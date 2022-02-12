using System;

public sealed class DelayedPayload : IPayloadProvider
{
    private readonly Func<IPayloadProvider> _getPayloadProvider;
    private bool _isInitialized;
    private IPayloadProvider _payloadProvider;

    public DelayedPayload(Func<IPayloadProvider> getPayloadProvider) => _getPayloadProvider = getPayloadProvider;

    public string Name
    {
        get
        {
            EnsureInitialized();
            return "Delayed - " + _payloadProvider.Name;
        }
    }
    
    public int Count => _isInitialized ? _payloadProvider.Count : 1;

    public bool IsFinished()
    {
        EnsureInitialized();
        return _payloadProvider.IsFinished();
    }

    public PayloadData GetNext()
    {
        EnsureInitialized();
        return _payloadProvider.GetNext();
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            _payloadProvider = _getPayloadProvider();
            _isInitialized = true;
        }
    }
}
