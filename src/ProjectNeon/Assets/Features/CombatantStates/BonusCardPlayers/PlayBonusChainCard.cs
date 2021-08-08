using System.Linq;

public class PlayBonusChainCard : TemporalStateBase, IBonusCardPlayer
{
    private readonly int _memberId;
    private readonly CardType _bonusCard;

    public override IStats Stats { get; } = new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();
    public override ITemporalState CloneOriginal() => new PlayBonusChainCard(_memberId, _bonusCard, Status);
    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    public PlayBonusChainCard(int memberId, CardType bonusCard, StatusDetail status)
        : base(TemporalStateMetadata.Unlimited(memberId, false, status))
    {
        _memberId = memberId;
        _bonusCard = bonusCard;
    }

    public Maybe<CardType> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot)
    {
        if (snapshot.PlayedCardHistory.None())
            return Maybe<CardType>.Missing();

        var member = snapshot.Members[_memberId];
        var teamType = member.TeamType;
        var teamCurrentTurnCards = snapshot.PlayedCardHistory.First().Where(x => x.Member.TeamType == teamType).Select(x => x.Member.Id);
        var result = teamCurrentTurnCards.All(id => id == _memberId)
            ? _bonusCard
            : Maybe<CardType>.Missing();
        return result;
    }
}