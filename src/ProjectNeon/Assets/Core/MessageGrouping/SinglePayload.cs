public sealed class SinglePayload : IPayloadProvider
{
    private PayloadData _payload;
    private bool _isFinished;
        
    public SinglePayload(object payload) : this(new PayloadData(payload)) {}
    public SinglePayload(string name, object payload) : this(name, new PayloadData(payload)) {}
    public SinglePayload(PayloadData payload) : this(payload.ToString(), payload) {}
    public SinglePayload(string name, PayloadData payload)
    {
        Name = name;
        _payload = payload;
    }

    public string Name { get; }
    public int Count => 1;
    public bool IsFinished() => _isFinished;

    public PayloadData GetNext()
    {
        _isFinished = true;
        return _payload;
    }
}
