public interface ITemporalState
{
    StatusTag Tag { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    Maybe<int> RemainingTurns { get; }
    Maybe<int> Amount { get; }
    IStats Stats { get; }
    ITemporalState CloneOriginal();
    IPayloadProvider OnTurnStart();
    IPayloadProvider OnTurnEnd();
}
