public class NoPayload : IPayloadProvider
{
    public string Name => "No Payload";
    public int Count => 0;
    public bool IsFinished() => true;
    public PayloadData GetNext() => null;
}
