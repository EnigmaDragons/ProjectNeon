public class NoPayload : IPayloadProvider
{
    public int Count => 0;
    public bool IsFinished() => true;
    public PayloadData GetNext() => null;
}
