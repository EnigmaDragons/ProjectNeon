using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutionPhase : OnMessage<ApplyBattleEffect, SpawnEnemy, CardResolutionFinished, CardActionAvoided>
{
    [SerializeField] private BattleUiVisuals ui;
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
    
    public IEnumerator Begin()
    {
        DevLog.Write($"Card Resolution Began");
        resolutionZone.Moves.ForEach(c => BattleLog.Write($"{c.Member.Name} played {c.Card.Name}{TargetDescription(c)}"));
        yield return ui.BeginResolutionPhase();
        yield return new WaitForSeconds(delay);
        ResolveNext();
    }

    private string TargetDescription(IPlayedCard c)
    {
        var seq = c.Card.ActionSequences[0];
        if (seq.Scope == Scope.One)
            return $" on {c.Targets[0].Members[0].Name}";
        if (seq.Group == Group.Self)
            return "";
        if (seq.Group == Group.Ally && seq.Scope == Scope.All)
            return " on all Allies";
        if (seq.Group == Group.Opponent && seq.Scope == Scope.All)
            return " on all Enemies";
        return "";
    }

    private void ResolveNext()
    {
        PerformConsciousnessUpdate();
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        if (IsDoneResolving)
            FinishResolutionPhase();
        else
            ProcessNextCardOrReaction();
    }

    private void PerformConsciousnessUpdate()
    {
        _unconsciousness.ProcessUnconsciousMembers(state)
            .ForEach(m => resolutionZone.ExpirePlayedCards(c => c.Member.Id == m.Id));
        _unconsciousness.ProcessRevivedMembers(state);
    }

    private void ProcessNextCardOrReaction()
    {
        if (_reactionCards.Any())
            StartCoroutine(ResolveNextReactionCard());
        else if (resolutionZone.HasMore)
            resolutionZone.BeginResolvingNext();
    }

    private void FinishResolutionPhase()
    {
        this.ExecuteAfterDelay(() =>
        {
            ui.EndResolutionPhase();
            Message.Publish(new ResolutionsFinished());
        }, delay);
    }

    private bool IsDoneResolving => state.BattleIsOver() || _reactionCards.None() && !resolutionZone.HasMore;
    
    protected override void Execute(ApplyBattleEffect msg)
    {
        var battleSnapshotBefore = state.GetSnapshot();
        ApplyEffectsWithRetargetingIfAllTargetsUnconscious(msg);
        var battleSnapshotAfter = state.GetSnapshot();
        
        var effectResolved = new EffectResolved(msg.Effect, msg.Source, msg.Target, battleSnapshotBefore, battleSnapshotAfter, msg.IsReaction);
        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(effectResolved)).ToList();

        var immediateReactions = reactions.Where(r => r.ReactionCard.IsMissing);
        immediateReactions.ForEach(r => _instantReactions.Enqueue(r));
        
        var reactionCards = reactions.Where(r => r.ReactionCard.IsPresent);
        reactionCards.ForEach(r => _reactionCards.Enqueue(r));
        
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    protected override void Execute(CardActionAvoided msg)
    {
        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(msg));
        reactions.ForEach(r => _reactionCards.Enqueue(r));
        Message.Publish(new Finished<CardActionAvoided>());
    }

    private void ApplyEffectsWithRetargetingIfAllTargetsUnconscious(ApplyBattleEffect msg)
    {
        if (msg.CanRetarget && msg.Target.Members.All(m => !m.IsConscious()))
        {
            DevLog.Write("Retargeting Battle Effect");
            var newTargets = state.GetPossibleConsciousTargets(msg.Source, msg.Group, msg.Scope);
            if (newTargets.Any())
            {
                AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, newTargets.Random(), partyAdventureState, state.PlayerState, state.Members));
                return;
            }
        }
        
        AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, msg.Target, partyAdventureState, state.PlayerState, state.Members));   
    }

    protected override void Execute(SpawnEnemy msg)
    {
        var member = enemies.Spawn(msg.Enemy);
        BattleLog.Write($"Spawned {member.Name}");
        Message.Publish(new Finished<SpawnEnemy>());
    }

    protected override void Execute(CardResolutionFinished msg) => StartCoroutine(FinishCard());

    private IEnumerator FinishCard()
    {
        if (_instantReactions.Any())
            BeginResolvingNextInstantReaction();
        else
        {
            yield return new WaitForSeconds(delay);
            currentResolvingCardZone.Clear();
            ResolveNext();
        }
    }

    private void BeginResolvingNextInstantReaction()
    {
        var r = _instantReactions.Dequeue();
        r.ReactionSequence.Perform(r.Source, r.Target, 0);
    }
    
    private IEnumerator ResolveNextReactionCard()
    {
        var r = _reactionCards.Dequeue();
        var isReactionCard = r.ReactionCard.IsPresent;

        if (!isReactionCard)
        {
            Log.Error("Should not be Queueing instant Effect Reactions. They should already be processed.");
            yield break;
        }

        var reactionCard = r.ReactionCard.Value;
        var card = new Card(state.GetNextCardId(), r.Source, reactionCard);
        reactionZone.PutOnBottom(card);
        currentResolvingCardZone.Set(card);
        yield return new WaitForSeconds(delay);
        if (reactionCard.IsPlayableBy(r.Source))
        {
            var expense = reactionCard.Cost.ResourcesSpent(r.Source);
            var gains = reactionCard.Gain.ResourcesGained(r.Source);
            var xAmountSpent = reactionCard.Cost.XAmountSpent(r.Source);
            var playedCard = new PlayedCardV2(r.Source, new[] {r.Target}, card, true, expense, gains, xAmountSpent);
            Message.Publish(new CardResolutionStarted(playedCard));
            r.Source.Apply(s => s.Lose(expense));
            r.Source.Apply(s => s.Gain(gains));
            reactionCard.ActionSequence.Perform(r.Source, r.Target, expense.Amount);
        }
        else 
            Message.Publish(new CardResolutionFinished(r.Source.Id));
    }
}
