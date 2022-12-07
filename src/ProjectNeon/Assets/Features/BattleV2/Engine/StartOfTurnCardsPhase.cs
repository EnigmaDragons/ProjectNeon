using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartOfTurnCardsPhase : OnMessage<BattleStateChanged, CardResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private CardResolutionZone resolutionZone;

    private BattleV2Phase _phase;
    private Queue<BonusCardAndOwner> _startOfTurnCards;

    public void BeginPlayingAllStartOfTurnCards()
    {
        _startOfTurnCards = new Queue<BonusCardAndOwner>();
        foreach (var member in state.MembersWithoutIds)
            foreach (var bonusCard in member.State.GetBonusStartOfTurnCards(state.GetSnapshot()))
                _startOfTurnCards.Enqueue(new BonusCardAndOwner { Member = member, BonusCard = bonusCard });
        PlayStartOfTurnCard();
    } 
    
    private void PlayStartOfTurnCard()
    {
        DevLog.Info($"Start of Turn Cards - Began Playing Next Start of Turn Card. {_startOfTurnCards.Count()} to play.");
        StartCoroutine(ExecuteAfterReactionsFinished(() =>
        {
            if (_startOfTurnCards.Count == 0)
                StartCoroutine(WaitForResolutionsFinished(BattleV2Phase.StartOfTurnCards));
            else
            {
                var bonusCard = _startOfTurnCards.Dequeue();
                if (!bonusCard.Member.IsConscious())
                    PlayNextCardInPhase();
                else
                    resolutionZone.PlayBonusCard(bonusCard.Member, bonusCard.BonusCard);
            }
        }));
    }
    
    private IEnumerator ExecuteAfterReactionsFinished(Action onFinished)
    {
        while (!resolutions.IsDoneResolving || state.Reactions.Any)
            yield return new WaitForSeconds(0.1f);
        onFinished();
    }
    
    private IEnumerator WaitForResolutionsFinished(BattleV2Phase phase)
    {
        while (!resolutions.IsDoneResolving)
            yield return new WaitForSeconds(0.1f);
        Message.Publish(new ResolutionsFinished(phase));
    }  
    
    private void PlayNextCardInPhase()
    {
        if (_phase == BattleV2Phase.StartOfTurnCards)
            PlayStartOfTurnCard();
    }

    protected override void Execute(BattleStateChanged msg) => _phase = state.Phase;
    protected override void Execute(CardResolutionFinished msg) => PlayNextCardInPhase();
}

[Serializable]
public class BonusCardAndOwner
{
    public Member Member;
    public BonusCardDetails BonusCard;
}