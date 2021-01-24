
public interface IBonusCardPlayer : ITemporalState
{
    Maybe<CardType> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot);
}