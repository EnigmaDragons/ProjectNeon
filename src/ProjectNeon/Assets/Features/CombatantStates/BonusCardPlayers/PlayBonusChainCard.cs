﻿using System.Linq;

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

    public Maybe<BonusCardDetails> GetBonusCardOnResolutionPhaseBegun(BattleStateSnapshot snapshot)
    {
        if (snapshot.PlayedCardHistory.None() || snapshot.NumCardPlaysRemaining > 0)
            return Maybe<BonusCardDetails>.Missing();

        var member = snapshot.Members[_memberId];
        var canChain = CanChain.Evaluate(member.Id, new IPlayedCard[0], snapshot.PlayedCardHistory.Last());
        var result = canChain
            ? new BonusCardDetails(_bonusCard, new ResourceQuantity { ResourceType = _bonusCard.Cost.ResourceType.Name, Amount = 0})
            : Maybe<BonusCardDetails>.Missing();
        return result;
    }

    public Maybe<BonusCardDetails> GetBonusCardOnStartOfTurnPhase(BattleStateSnapshot snapshot)
        => Maybe<BonusCardDetails>.Missing();
}
