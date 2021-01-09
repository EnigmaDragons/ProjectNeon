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
    private readonly Queue<ProposedReaction> _reactions = new Queue<ProposedReaction>();
    
    public IEnumerator Begin()
    {
        BattleLog.Write($"Card Resolution Began");
        yield return ui.BeginResolutionPhase();
        yield return new WaitForSeconds(delay);
        ResolveNext();
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
        if (_reactions.Any())
            StartCoroutine(ResolveNextReaction());
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

    private bool IsDoneResolving => state.BattleIsOver() || _reactions.None() && !resolutionZone.HasMore;
    
    protected override void Execute(ApplyBattleEffect msg)
    {
        var battleSnapshotBefore = state.GetSnapshot();
        ApplyEffectsWithRetargetingIfAllTargetsUnconscious(msg);
        var battleSnapshotAfter = state.GetSnapshot();
        var effectResolved = new EffectResolved(msg.Effect, msg.Source, msg.Target, battleSnapshotBefore, battleSnapshotAfter, msg.IsReaction);

        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(effectResolved));
        reactions.ForEach(r => _reactions.Enqueue(r));
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    protected override void Execute(CardActionAvoided msg)
    {
        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(msg));
        reactions.ForEach(r => _reactions.Enqueue(r));
        Message.Publish(new Finished<CardActionAvoided>());
    }

    private void ApplyEffectsWithRetargetingIfAllTargetsUnconscious(ApplyBattleEffect msg)
    {
        if (msg.CanRetarget && msg.Target.Members.All(m => !m.IsConscious()))
        {
            Log.Info("Retargeting Battle Effect");
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
        enemies.Spawn(msg.Enemy);
        Message.Publish(new Finished<SpawnEnemy>());
    }

    protected override void Execute(CardResolutionFinished msg) => StartCoroutine(FinishCard());

    private IEnumerator FinishCard()
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"Clearing {currentResolvingCardZone.Count} cards from Current Zone");
        currentResolvingCardZone.Clear();
        ResolveNext();
    }

    private IEnumerator ResolveNextReaction()
    {
        var r = _reactions.Dequeue();
        var card = new Card(state.GetNextCardId(), r.Source, r.Reaction);
        reactionZone.PutOnBottom(card);
        currentResolvingCardZone.Set(card);
        yield return new WaitForSeconds(delay);
        var cost = r.Reaction.Cost;
        var gain = r.Reaction.Gain;
        if (r.Reaction.IsPlayableBy(r.Source))
        {
            Message.Publish(new CardResolutionStarted { Originator = r.Source.Id });
            var expense = cost.ResourcesSpent(r.Source);
            var gains = gain.ResourcesGained(r.Source);
            r.Source.Apply(s => s.Lose(expense));
            r.Source.Apply(s => s.Gain(gains));
            r.Reaction.ActionSequence.Perform(r.Source, r.Target, expense.Amount);
        }
        else 
            Message.Publish(new CardResolutionFinished());
    }
}
