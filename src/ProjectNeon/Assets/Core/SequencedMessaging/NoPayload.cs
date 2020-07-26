public class NoPayload : IPayloadProvider
{
    public bool IsFinished() => true;
    public PayloadData GetNext() => null;
}