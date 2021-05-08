using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutions : OnMessage<ApplyBattleEffect, SpawnEnemy, DespawnEnemy, CardResolutionFinished, CardActionPrevented>
{
    [SerializeField] private BattleState state;
    [SerializeField] private PartyAdventureState partyAdventureState;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private CardPlayZone reactionZone;
    [SerializeField] private CardPlayZone currentResolvingCardZone;
    [SerializeField] private EnemyVisualizerV2 enemies;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);

    private readonly BattleUnconsciousnessChecker _unconsciousness = new BattleUnconsciousnessChecker();
    private readonly Queue<ProposedReaction> _instantReactions = new Queue<ProposedReaction>();
    private readonly Queue<ProposedReaction> _reactionCards = new Queue<ProposedReaction>();
    private bool _resolvingEffect;
    
    private void ResolveNext()
    {
        PerformConsciousnessUpdate();
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        
        if (_reactionCards.Any())
            StartCoroutine(ResolveNextReactionCard());
        else if (resolutionZone.HasMore)
            resolutionZone.BeginResolvingNext();
    }

    public void PerformConsciousnessUpdate()
    {
        _unconsciousness.ProcessUnconsciousMembers(state)
            .ForEach(m => resolutionZone.ExpirePlayedCards(c => c.Member.Id == m.Id)); // Still needed?
        _unconsciousness.ProcessRevivedMembers(state);
    }

    public bool IsDoneResolving => state.BattleIsOver() || _reactionCards.None() && resolutionZone.IsDone && _instantReactions.None() && !_resolvingEffect;
    
    protected override void Execute(ApplyBattleEffect msg)
    {
        var battleSnapshotBefore = state.GetSnapshot();
        ApplyEffectsWithRetargetingIfAllTargetsUnconscious(msg);
        var battleSnapshotAfter = state.GetSnapshot();
        
        var effectResolved = new EffectResolved(msg.Effect, msg.Source, msg.Target, battleSnapshotBefore, battleSnapshotAfter, msg.IsReaction, msg.Card, msg.Preventions);
        var reactions = state.Members
            .Where(x => x.Value.CanReact())
            .Select(x => x.Value)
            .SelectMany(v => v.State.GetReactions(effectResolved)).ToList();

        var immediateReactions = reactions.Where(r => r.ReactionCard.IsMissing);
        immediateReactions.ForEach(r => _instantReactions.Enqueue(r));
        
        var reactionCards = reactions.Where(r => r.ReactionCard.IsPresent);
        reactionCards.ForEach(r => _reactionCards.Enqueue(r));
        
        state.CleanupExpiredMemberStates();
        
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    private static readonly HashSet<EffectType> StealthBreakingEffectTypes = new HashSet<EffectType>(new []
    {
        EffectType.AttackFormula, 
        EffectType.Kill, 
        EffectType.DamageOverTimeFormula,
        EffectType.MagicAttackFormula, 
    });
    
    protected override void Execute(CardActionPrevented msg)
    {
        if (msg.ToDecrement == TemporalStatType.Blind)
            BattleLog.Write($"{msg.Source.Name} was blinded, so their attack missed.");
        Message.Publish(new PlayRawBattleEffect("MissedText", Vector3.zero));
        msg.Source.State.Adjust(msg.ToDecrement, -1);
        Message.Publish(new Finished<CardActionPrevented>());
    }
    
    private void ApplyEffectsWithRetargetingIfAllTargetsUnconscious(ApplyBattleEffect msg)
    {
        if (StealthBreakingEffectTypes.Contains(msg.Effect.EffectType))
            msg.Source.State.ExitStealth();
        
        if (msg.CanRetarget && msg.Target.Members.All(m => !m.IsConscious()))
        {
            DevLog.Write("Retargeting Battle Effect");
            var newTargets = state.GetPossibleConsciousTargets(msg.Source, msg.Group, msg.Scope);
            if (newTargets.Any())
            {
                AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, newTargets.Random(), msg.Card, msg.XPaidAmount, 
                    partyAdventureState, state.PlayerState, state.Members, state.PlayerCardZones, msg.Preventions));
                return;
            }
        }
        
        AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, msg.Target, msg.Card, msg.XPaidAmount, 
            partyAdventureState, state.PlayerState, state.Members, state.PlayerCardZones, msg.Preventions));   
    }

    protected override void Execute(SpawnEnemy msg)
    {
        var member = enemies.Spawn(msg.Enemy.GetEnemy(state.Stage), msg.Offset);
        BattleLog.Write($"Spawned {member.Name}");
        Message.Publish(new Finished<SpawnEnemy>());
    }
    
    protected override void Execute(DespawnEnemy msg)
    {
        enemies.Despawn(msg.Member);
        BattleLog.Write($"Despawned {msg.Member.Name}");
        Message.Publish(new Finished<DespawnEnemy>());
    }

    protected override void Execute(CardResolutionFinished msg) => StartCoroutine(FinishEffect());

    private IEnumerator FinishEffect()
    {
        state.CleanupExpiredMemberStates();
        resolutionZone.ExpirePlayedCards(c => !state.Members.ContainsKey(c.MemberId()));
        if (_instantReactions.Any())
        {
            _resolvingEffect = false;
            BeginResolvingNextInstantReaction();
        }
        else
        {
            yield return new WaitForSeconds(delay);
            _resolvingEffect = false;
            resolutionZone.OnCardResolutionFinished();
            ResolveNext();
        }
    }

    private void BeginResolvingNextInstantReaction()
    {
        var r = _instantReactions.Dequeue();
        r.ReactionSequence.Perform(r.Name, r.Source, r.Target, ResourceQuantity.None);
    }
    
    private IEnumerator ResolveNextReactionCard()
    {
        _resolvingEffect = true;
        var r = _reactionCards.Dequeue();
        var isReactionCard = r.ReactionCard.IsPresent;
        if (!isReactionCard)
        {
            _resolvingEffect = false;
            Log.Error("Should not be Queueing instant Effect Reactions. They should already be processed.");
            yield break;
        }

        if (!state.Members.ContainsKey(r.Source.Id))
        {
            StartCoroutine(FinishEffect());
            yield break;
        }

        var reactionCard = r.ReactionCard.Value;
        if (reactionCard.IsPlayableBy(r.Source))
        {
            BattleLog.Write($"{r.Source.Name} has reacted with {reactionCard.Name}");
            Message.Publish(new PlayRawBattleEffect("ReactionText"));
            var card = new Card(state.GetNextCardId(), r.Source, reactionCard);
            reactionZone.PutOnBottom(card);
            currentResolvingCardZone.Set(card);
            yield return new WaitForSeconds(delay);
            
            var resourceCalculations = r.Source.CalculateResources(reactionCard);
            var playedCard = new PlayedCardV2(r.Source, new[] {r.Target}, card, true, resourceCalculations);
            Message.Publish(new CardResolutionStarted(playedCard));
            r.Source.Apply(s => s.Lose(resourceCalculations.PaidQuantity));
            r.Source.Apply(s => s.Gain(resourceCalculations.GainedQuantity));
            reactionCard.ActionSequence.Perform(r.Name, r.Source, r.Target, resourceCalculations.XAmountQuantity);
        }
        else
        {
            BattleLog.Write($"{r.Source.Name} could not afford reaction card {reactionCard.Name}");
            this.ExecuteAfterDelay(() => StartCoroutine(FinishEffect()), 0.1f);
        }
    }
}
