using System.Linq;

public class PlayBonusCardAfterNoCardPlayedInXTurns : TemporalStateBase, IBonusCardPlayer
{
    private readonly StringReference _className;
    private readonly CardType _bonusCard;
    private readonly int _numTurns;

    public StatusDetail Status { get; }
    public override Maybe<string> CustomStatusText => Status.CustomText;
    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new PlayBonusCardAfterNoCardPlayedInXTurns(_className, _bonusCard, _numTurns, Status);
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    public PlayBonusCardAfterNoCardPlayedInXTurns(StringReference className, CardType bonusCard, int numTurns, StatusDetail status)
        : base(TemporalStateMetadata.Indefinite(false, status.Tag))
    {
        Status = status;
        _className = className;
        _bonusCard = bonusCard;
        _numTurns = numTurns;
    }

    public Maybe<CardType> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot)
    {
        return snapshot.PlayedCardHistory.Count < _numTurns
               || snapshot.PlayedCardHistory
                    .TakeLast(_numTurns)
                    .SelectMany(x => x)
                    .Any(x => _className.Value.Equals(x.LimitedToClass.Select(c => c.Name, "")))
            ? Maybe<CardType>.Missing()
            : _bonusCard;
    }
}
