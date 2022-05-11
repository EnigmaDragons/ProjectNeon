public interface IPayloadProvider
{
    string Name { get; }
    int Count { get; }
    bool IsFinished();
    PayloadData GetNext();
}
