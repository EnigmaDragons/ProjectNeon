public interface IPayloadProvider
{
    bool IsFinished();
    PayloadData GetNext();
}