using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutionPhase : OnMessage<ApplyBattleEffect, SpawnEnemy, CardResolutionFinished>
{
    [SerializeField] private BattleUiVisuals ui;
    [SerializeField] private BattleState state;
    [SerializeField] private PartyAdventureState partyAdventureState;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private CardPlayZone reactionZone;
    [SerializeField] private EnemyVisualizerV2 enemies;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);

    private readonly BattleUnconsciousnessChecker _unconsciousness = new BattleUnconsciousnessChecker();
    private readonly Queue<ProposedReaction> _reactions = new Queue<ProposedReaction>();
    
    public IEnumerator Begin()
    {
        BattleLog.Write($"Card Resolution Began");
        yield return ui.BeginResolutionPhase();
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
            this.ExecuteAfterDelay(() => StartCoroutine(ResolveNextReaction()), delay);
        else if (resolutionZone.HasMore)
            this.ExecuteAfterDelay(() => resolutionZone.BeginResolvingNext(), delay);
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
        Message.Publish(new CardResolutionFinished());
    }

    protected override void Execute(CardResolutionFinished msg) => ResolveNext();

    private IEnumerator ResolveNextReaction()
    {
        var r = _reactions.Dequeue();
        reactionZone.PutOnBottom(new Card(state.GetNextCardId(), r.Source, r.Reaction));
        yield return new WaitForSeconds(delay);
        var cost = r.Reaction.Cost;
        var gain = r.Reaction.Gain;
        if (r.Reaction.IsPlayableBy(r.Source))
        {
            var expense = cost.ResourcesSpent(r.Source);
            var gains = gain.ResourcesGained(r.Source);
            r.Source.Apply(s => s.Lose(expense));
            r.Source.Apply(s => s.Gain(gains));
            r.Reaction.ActionSequence.Perform(r.Source, r.Target, expense.Amount);
        }
    }
}
