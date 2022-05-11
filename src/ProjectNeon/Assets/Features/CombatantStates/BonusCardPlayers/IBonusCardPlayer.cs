
public interface IBonusCardPlayer : ITemporalState
{
    Maybe<BonusCardDetails> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot);
}
