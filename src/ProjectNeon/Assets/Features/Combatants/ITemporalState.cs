public interface ITemporalState
{
    IStats Stats { get; }
    StatusTag Tag { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    IPayloadProvider OnTurnStart();
    IPayloadProvider OnTurnEnd();
    
    ITemporalState CloneOriginal();
}
