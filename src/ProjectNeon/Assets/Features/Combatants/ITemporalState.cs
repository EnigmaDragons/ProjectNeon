public interface ITemporalState
{
    IStats Stats { get; }
    bool IsActive { get; }
    void AdvanceTurn();
}
