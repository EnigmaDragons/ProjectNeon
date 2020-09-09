using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutionPhase : OnMessage<ApplyBattleEffect, SpawnEnemy, CardResolutionFinished>
{
    [SerializeField] private BattleUiVisuals ui;
    [SerializeField] private BattleState state;
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
        _unconsciousness.ProcessUnconsciousMembers(state)
            .ForEach(m => resolutionZone.ExpirePlayedCards(c => c.Member.Id == m.Id));
        if (reactionZone.Count > 0)
            reactionZone.Clear();
        if (_reactions.Any())
            StartCoroutine(ResolveNextReaction());
        else if (!state.BattleIsOver() && resolutionZone.HasMore)
            this.ExecuteAfterDelay(() => resolutionZone.BeginResolvingNext(), delay);
        else
        {
            ui.EndResolutionPhase();
            Message.Publish(new ResolutionsFinished());
        }
    }

    protected override void Execute(ApplyBattleEffect msg)
    {
        var battleSnapshotBefore = state.GetSnapshot();
        ApplyEffectsWithRetargetingIsAllTargetsUnconscious(msg);
        var battleSnapshotAfter = state.GetSnapshot();
        var effectResolved = new EffectResolved(msg.Effect, msg.Source, msg.Target, battleSnapshotBefore, battleSnapshotAfter);

        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(effectResolved));
        reactions.ForEach(r => _reactions.Enqueue(r));
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    private void ApplyEffectsWithRetargetingIsAllTargetsUnconscious(ApplyBattleEffect msg)
    {
        if (msg.CanRetarget && msg.Target.Members.All(m => !m.IsConscious()))
        {
            var newTargets = state.GetPossibleConsciousTargets(msg.Source, msg.Group, msg.Scope);
            if (newTargets.Any())
            {
                AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, newTargets.Random(), state.PlayerState, state.Members));
                return;
            }
        }
        
        AllEffects.Apply(msg.Effect, new EffectContext(msg.Source, msg.Target, state.PlayerState, state.Members));   
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
