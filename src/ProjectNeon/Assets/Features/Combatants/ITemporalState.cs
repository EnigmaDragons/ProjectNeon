public interface ITemporalState
{
    StatusTag Tag { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    IStats Stats { get; }
    ITemporalState CloneOriginal();
    IPayloadProvider OnTurnStart();
    IPayloadProvider OnTurnEnd();
    
}
