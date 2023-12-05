using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStatusEffects : OnMessage<StatusEffectResolved, PerformAction, CardResolutionFinished, CardAndEffectsResolutionFinished>
{
    [SerializeField] private BattleState state;
    [SerializeField] private FloatReference delay = new FloatReference(0.5f);

    private BattleReactions Reactions => state.Reactions;
    
    private readonly Queue<Member> _membersToProcess = new Queue<Member>();
    private Maybe<Member> _currentMember;
    
    private readonly Queue<IPayloadProvider> _currentMemberEffects = new Queue<IPayloadProvider>();
    private readonly HashSet<int> _processedCardIds = new HashSet<int>();

    private bool _isProcessing;
    private bool _isProcessingStartOfTurn;
    private bool _isProcessingEndOfTurn;
    
    public void ProcessStartOfTurnEffects()
    {
        _isProcessing = true;
        _isProcessingStartOfTurn = true;
        foreach (var member in state.Members.Values.Where(x => x.IsConscious())) 
            _membersToProcess.Enqueue(member);
        ResolveNext(nameof(ProcessStartOfTurnEffects));
    }
    
    public void ProcessEndOfTurnEffects()
    {
        _isProcessing = true;
        _isProcessingEndOfTurn = true;
        foreach (var m in state.Members.Where(x => x.Value.IsConscious())) 
            _membersToProcess.Enqueue(m.Value);
        this.ExecuteAfterDelay(() => ResolveNext(nameof(ProcessEndOfTurnEffects)), delay);
    }

    private void ProcessCardsStartOfturn()
    {
        foreach (var card in state.PlayerCardZones.HandZone.Cards.Concat(state.PlayerCardZones.DrawZone.Cards).Concat(state.PlayerCardZones.DiscardZone.Cards))
        {
            card.OnTurnStart();
            card.CleanExpiredStates();
        }
    }
    
    private void ProcessCardsEndOfturn()
    {
        foreach (var card in state.PlayerCardZones.HandZone.Cards.Concat(state.PlayerCardZones.DrawZone.Cards).Concat(state.PlayerCardZones.DiscardZone.Cards))
        {
            card.OnTurnEnd();
            card.CleanExpiredStates();
        }
    }

    private void ResolveNext(string debugCallerName)
    {
        if (!_isProcessing)
            return;
        
        Log.Info($"Status Effects Resolve Next - {debugCallerName}");
        if (Reactions.AnyReactionEffects && Reactions.TryResolveNextInstantReaction(state.Members))
            return;
        else if (Reactions.AnyReactionCards)
            Message.Publish(new ResolveReactionCards());
        else if (_currentMemberEffects.Any())
            ResolveNextStatusEffect();
        else if (_membersToProcess.Any())
            ResolveNextMemberStatusEffects();
        else if (!Reactions.Any)
        {
            if (_currentMember.IsPresent)
                _currentMember.Value.State.CleanExpiredStates();
            _currentMember = Maybe<Member>.Missing();
            if ((_isProcessingStartOfTurn || _isProcessingEndOfTurn) && !MessageGroup.IsClear)
                WaitForWrapup();
            else if (_isProcessingStartOfTurn)
            {
                ProcessCardsStartOfturn();
                _isProcessing = false;
                _isProcessingStartOfTurn = false;
                Message.Publish(new StartOfTurnEffectsStatusResolved());
            }
            else if (_isProcessingEndOfTurn)
            {
                ProcessCardsEndOfturn();
                _isProcessing = false;
                _isProcessingEndOfTurn = false;
                Message.Publish(new EndOfTurnStatusEffectsResolved());   
            }
            else
                _isProcessing = false;
        }
        else
            WaitForWrapup();
    }

    private void WaitForWrapup()
    {
        Log.Info("Status Effect - Waiting to Wrapup - MessageGroup" + MessageGroup.CurrentName);
        Async.ExecuteAfterDelay(50, () => ResolveNext("Revolve Next after Delay - Waiting for Wrapup"));
    }
    
    private void ResolveNextStatusEffect()
    {
        var e = _currentMemberEffects.Dequeue();
        DevLog.Write($"Resolve Next Status Effect - Effect is Finished {e.IsFinished()}");
        if (!e.IsFinished())
        {
            var member = _currentMember.Value;
            var battleSnapshotBefore = state.GetSnapshot();

            MessageGroup.Start(e, () => 
            {
                var battleSnapshotAfter = state.GetSnapshot();
                var effectResolved = new EffectResolved(true, 
                    true,
                    EffectData.Nothing, 
                    member, // Replace with Status Originator
                    new Single(member), 
                    battleSnapshotBefore,
                    battleSnapshotAfter, 
                    false,
                    Maybe<Card>.Missing(),
                    Array.Empty<Card>(),
                    Array.Empty<Card>(),
                    new UnpreventableContext(),
                    ReactionTimingWindow.FirstCause,
                    state.PlayerCardZones);
                Message.Publish(new StatusEffectResolved(member, effectResolved));
            });
        }
        else
        {
            this.ExecuteAfterDelay(() =>
            {
                _currentMember.Value.State.CleanExpiredStates();
                ResolveNext("Resolve Next Status Effect - Effect already Finished");
            }, delay);
        }
    }
    
    private void ResolveNextMemberStatusEffects()
    {
        var member = _membersToProcess.Dequeue();
        _currentMember = member;
        
        if (!member.State.HasAnyTemporalStates)
        {
            ResolveNext("Go To Next Member");
        }
        else
        {
            var effects = _isProcessingStartOfTurn
                ? member.State.GetTurnStartEffects()
                : member.State.GetTurnEndEffects();
            if (effects.Length > 0)
                DevLog.Write($"Resolving {effects.Length} Status Effects for {member.NameTerm.ToEnglish()}");
            effects.Where(e => !e.IsFinished())
                .ForEach(e => _currentMemberEffects.Enqueue(e));
            ResolveNext("Enqueue Next Member Effects");
        }
    }

    protected override void Execute(StatusEffectResolved msg)
    {
        DevLog.Write("Status Effect Resolved - Adding Status Effect Reactions");
        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(msg.EffectResolved, state.Phase == BattleV2Phase.HastyEnemyCards || state.Phase == BattleV2Phase.PlayCards || state.Phase == BattleV2Phase.EnemyCards)).ToArray();
        Reactions.Enqueue(reactions);
        
        msg.Member.State.CleanExpiredStates();
        ResolveNext(nameof(StatusEffectResolved));
    }

    protected override void Execute(PerformAction msg)
    {
        msg.Action();
        Message.Publish(new Finished<PerformAction>());
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        if (_processedCardIds.Contains(msg.PlayedCardId))
            return;

        DevLog.Write($"Card Resolution Finished {msg.CardName} {msg.CardId} {msg.PlayedCardId}");
        _processedCardIds.Add(msg.PlayedCardId);
        ResolveNext("Card Resolution Finished");
    }

    protected override void Execute(CardAndEffectsResolutionFinished msg) => ResolveNext("Resolve Reaction Cards Finished");
}
