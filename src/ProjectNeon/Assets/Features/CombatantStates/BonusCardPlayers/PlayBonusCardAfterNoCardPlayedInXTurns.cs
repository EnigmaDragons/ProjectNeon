using System.Linq;

public class PlayBonusCardAfterNoCardPlayedInXTurns : TemporalStateBase, IBonusCardPlayer
{
    private readonly int _memberId;
    private readonly CardType _bonusCard;
    private readonly int _numTurns;

    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new PlayBonusCardAfterNoCardPlayedInXTurns(_memberId, _bonusCard, _numTurns, Status);
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    public PlayBonusCardAfterNoCardPlayedInXTurns(int memberId, CardType bonusCard, int numTurns, StatusDetail status)
        : base(TemporalStateMetadata.Unlimited(false, status))
    {
        _memberId = memberId;
        _bonusCard = bonusCard;
        _numTurns = numTurns;
    }

    public Maybe<CardType> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot)
    {
        return snapshot.PlayedCardHistory.Count < _numTurns
               || snapshot.PlayedCardHistory
                    .TakeLast(_numTurns)
                    .SelectMany(x => x)
                    .Any(x => x.Member.Id == _memberId)
            ? Maybe<CardType>.Missing()
            : _bonusCard;
    }
}
