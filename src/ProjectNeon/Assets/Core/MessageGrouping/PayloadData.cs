using System;

public sealed class PayloadData
{
    public object Payload { get; }
    public Type Finished { get; }
    public Func<object, bool> FinishedCondition { get; }

    public static PayloadData ExactMatch<T>(T payload) 
        => new PayloadData(payload,  typeof(Finished<>).MakeGenericType(new Type[] { payload.GetType() }), 
            finished => finished is Finished<T> finished1 && payload.Equals(finished1.Message));
    public PayloadData(object payload) : this(payload, typeof(Finished<>).MakeGenericType(new Type[] { payload.GetType() }), _ => true) {}
    public PayloadData(object payload, Type finished, Func<object, bool> finishedCondition)
    {
        Payload = payload;
        Finished = finished;
        FinishedCondition = finishedCondition;
    }
}