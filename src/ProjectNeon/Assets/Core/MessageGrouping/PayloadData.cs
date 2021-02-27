using System;

public sealed class PayloadData
{
    public object Payload { get; }
    public Type Finished { get; }

    public PayloadData(object payload) : this(payload, typeof(Finished<>).MakeGenericType(new Type[] { payload.GetType() })) {}
    public PayloadData(object payload, Type finished)
    {
        Payload = payload;
        Finished = finished;
    }
}