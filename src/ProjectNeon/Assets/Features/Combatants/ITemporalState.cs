public interface ITemporalState
{
    IStats Stats { get; }
    StatusTag Tag { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    void OnTurnStart();
    void OnTurnEnd();
}
