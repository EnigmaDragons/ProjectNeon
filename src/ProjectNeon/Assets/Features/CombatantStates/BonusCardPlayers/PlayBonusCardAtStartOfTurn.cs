public class PlayBonusCardAtStartOfTurn : TemporalStateBase, IBonusCardPlayer
{
    private readonly int _memberId;
    private readonly CardType _bonusCard;

    public override IStats Stats => new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new PlayBonusCardAtStartOfTurn(_memberId, Tracker.Metadata.MaxDurationTurns, _bonusCard, Status);

    public override IPayloadProvider OnTurnStart() => new NoPayload();

    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    public PlayBonusCardAtStartOfTurn(int memberId, int duration, CardType bonusCard, StatusDetail status)
        : base(TemporalStateMetadata.ForDuration(memberId, duration, false, status))
    {
        _memberId = memberId;
        _bonusCard = bonusCard;
    }

    public Maybe<BonusCardDetails> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot)
        => Maybe<BonusCardDetails>.Missing();

    public Maybe<BonusCardDetails> GetBonusCardOnStartOfTurnPhase(BattleStateSnapshot snapshot)
    {
        Tracker.AdvanceTurn();
        return new BonusCardDetails(_bonusCard, new ResourceQuantity { ResourceType = _bonusCard.Cost.ResourceType.Name, Amount = _bonusCard.Cost.BaseAmount });
    }
}