public interface ITemporalState
{
    IStats Stats { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    void AdvanceTurn();
}
