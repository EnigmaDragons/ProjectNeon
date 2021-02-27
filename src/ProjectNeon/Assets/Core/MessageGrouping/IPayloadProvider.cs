public interface IPayloadProvider
{
    int Count { get; }
    bool IsFinished();
    PayloadData GetNext();
}
