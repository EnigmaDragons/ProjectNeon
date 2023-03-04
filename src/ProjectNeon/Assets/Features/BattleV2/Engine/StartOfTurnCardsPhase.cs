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
    
    private bool _debugLoggingEnabled = false;

    public void BeginPlayingAllStartOfTurnCards()
    {
        _phase = BattleV2Phase.StartOfTurnCards;
        _startOfTurnCards = new Queue<BonusCardAndOwner>();
        foreach (var member in state.MembersWithoutIds)
            foreach (var bonusCard in member.State.GetBonusStartOfTurnCards(state.GetSnapshot()))
                _startOfTurnCards.Enqueue(new BonusCardAndOwner { Member = member, BonusCard = bonusCard });
        PlayStartOfTurnCard();
    } 
    
    private void PlayStartOfTurnCard()
    {
        if (_debugLoggingEnabled)
            DebugLog($"Start of Turn Cards - Began Playing Next Start of Turn Card. {_startOfTurnCards.Count} to play.");
        this.SafeCoroutineOrNothing(ExecuteAfterReactionsFinished(() =>
        {
            if (_startOfTurnCards.Count == 0)
                this.SafeCoroutineOrNothing(WaitForResolutionsFinished(BattleV2Phase.StartOfTurnCards));
            else
            {
                var bonusCard = _startOfTurnCards.Dequeue();
                if (!bonusCard.BonusCard.Card.IsPlayableBy(bonusCard.Member, state.Party, 1))
                {
                    if (_debugLoggingEnabled)
                        DebugLog("Not Playing Bonus Card");
                    PlayNextCardInPhase();
                }
                else
                {
                    if (_debugLoggingEnabled)
                        DebugLog("Playing Bonus Card");
                    resolutionZone.PlayBonusCard(bonusCard.Member, bonusCard.BonusCard);
                }
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
        if (_debugLoggingEnabled)
            DebugLog($"PlayNextCardInPhase - Phase {_phase}");
        if (_phase == BattleV2Phase.StartOfTurnCards)
            PlayStartOfTurnCard();
    }

    protected override void Execute(BattleStateChanged msg)
    {
        if (_debugLoggingEnabled)
            DebugLog("Phase is " + msg.State.Phase);
        _phase = state.Phase;
    }

    protected override void Execute(CardResolutionFinished msg) => PlayNextCardInPhase();

    private void DebugLog(string msg)
    {
        if (_debugLoggingEnabled)
            DevLog.Info($"StartOfTurnCardsPhase - {msg}");
    }
}

[Serializable]
public class BonusCardAndOwner
{
    public Member Member;
    public BonusCardDetails BonusCard;
}
