public sealed class SinglePayload : IPayloadProvider
{
    private PayloadData _payload;
    private bool _isFinished;
        
    public SinglePayload(object payload) : this(new PayloadData(payload)) {}
    public SinglePayload(PayloadData payload) => _payload = payload;

    public bool IsFinished() => _isFinished;

    public PayloadData GetNext()
    {
        _isFinished = true;
        return _payload;
    }
}