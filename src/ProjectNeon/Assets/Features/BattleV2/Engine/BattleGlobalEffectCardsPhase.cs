using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleGlobalEffectCardsPhase : OnMessage<BattleStateChanged, CardResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private CurrentGlobalEffects globalEffects;

    private BattleV2Phase _phase;
    private Member _globalMember;
    private Queue<CardType[]> _globalCardsToPlay;

    public void BeginPlayingAllGlobalEffectCards()
    {
        _globalMember = new Member(AllGlobalEffects.GlobalEffectMemberId, "Global Effect Member", "Game Master", MemberMaterialType.Metallic, TeamType.Enemies, new StatAddends(new Dictionary<string, float>() { { StatType.Damagability.ToString(), 1f }}), BattleRole.Specialist, StatType.Leadership);
        _globalCardsToPlay = _globalCardsToPlay = new Queue<CardType[]>(globalEffects.StartOfBattleCards.ToArray());
        PlayNextGlobalEffectCard();
    } 
    
    private void PlayNextGlobalEffectCard()
    {
        DevLog.Info($"Global Effect Cards - Began Playing Next Global Effect Card. {_globalCardsToPlay.Count()} to play.");
        StartCoroutine(ExecuteAfterReactionsFinished(() =>
        {
            if (_globalCardsToPlay.Count == 0)
                StartCoroutine(WaitForResolutionsFinished(BattleV2Phase.GlobalEffectCards));
            else
            {
                var cardType = _globalCardsToPlay.Dequeue().Random();
                var targets = cardType.ActionSequences.Select<CardActionSequence, Target>(x =>
                {
                    if (x.Group == Group.All && x.Scope == Scope.All)
                        return new Multiple(state.MembersWithoutIds.Where(m => m.IsConscious()).ToArray());
                    if (x.Group == Group.Ally && x.Scope == Scope.All)
                        return new Multiple(state.MembersWithoutIds.Where(m => m.TeamType == TeamType.Party && m.IsConscious()).ToArray());
                    if (x.Group == Group.Opponent && x.Scope == Scope.All)
                        return new Multiple(state.MembersWithoutIds.Where(m => m.TeamType == TeamType.Enemies && m.IsConscious()).ToArray());
                    if (x.Group == Group.All && (x.Scope == Scope.One || x.Scope == Scope.Random || x.Scope == Scope.RandomExceptTarget)) //doesn't properly implement RandomExceptTarget
                        return new Single(state.MembersWithoutIds.Where(m => m.IsConscious()).Random());
                    if (x.Group == Group.Ally && (x.Scope == Scope.One || x.Scope == Scope.Random || x.Scope == Scope.RandomExceptTarget)) //doesn't properly implement RandomExceptTarget
                        return new Single(state.MembersWithoutIds.Where(m => m.TeamType == TeamType.Party && m.IsConscious()).Random());
                    if (x.Group == Group.Opponent && (x.Scope == Scope.One || x.Scope == Scope.Random || x.Scope == Scope.RandomExceptTarget)) //doesn't properly implement RandomExceptTarget
                        return new Single(state.MembersWithoutIds.Where(m => m.TeamType == TeamType.Enemies && m.IsConscious()).Random());
                    return new NoTarget();
                }).ToArray();
                var card = new Card(state.GetNextCardId(), _globalMember, cardType);
                resolutionZone.PlayImmediately(new PlayedCardV2(_globalMember, targets, card, false));
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
        if (_phase == BattleV2Phase.GlobalEffectCards)
            PlayNextGlobalEffectCard();
    }

    protected override void Execute(BattleStateChanged msg) => _phase = state.Phase;
    protected override void Execute(CardResolutionFinished msg) => PlayNextCardInPhase();
}