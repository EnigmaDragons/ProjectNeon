
using JetBrains.Annotations;

public interface IBonusCardPlayer : ITemporalState
{
    Maybe<BonusCardDetails> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot);
    Maybe<BonusCardDetails> GetBonusCardOnStartOfTurnPhase(BattleStateSnapshot snapshot);
}
