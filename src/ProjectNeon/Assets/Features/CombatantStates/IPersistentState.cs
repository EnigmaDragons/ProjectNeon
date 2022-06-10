public interface IPersistentState
{
    IPayloadProvider OnTurnStart();
    IPayloadProvider OnTurnEnd();
}