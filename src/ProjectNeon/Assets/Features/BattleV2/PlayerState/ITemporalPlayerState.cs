public interface ITemporalPlayerState
{
    IPlayerStats PlayerStats { get; }
    bool IsDebuff { get; }
    bool IsActive { get; }
    void OnTurnStart();
    void OnTurnEnd();
}