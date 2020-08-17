using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleResolutionPhase : OnMessage<ApplyBattleEffect, CardResolutionFinished>
{
    [SerializeField] private BattleUiVisuals ui;
    [SerializeField] private BattleState state;
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);
    
    [ReadOnly, SerializeField] private List<Member> _unconscious = new List<Member>();
    private readonly Queue<ProposedReaction> _reactions = new Queue<ProposedReaction>();
    
    public IEnumerator Begin()
    {
        BattleLog.Write($"Card Resolution Began");
        yield return ui.BeginResolutionPhase();
        ResolveNext();
    }

    private void ResolveNext()
    {
        CheckForUnconsciousMembers();
        if (_reactions.Any())
            StartCoroutine(ResolveAllReactions());
        else if (!state.BattleIsOver() && resolutionZone.HasMore)
            StartCoroutine(resolutionZone.ResolveNext(delay));
        else
        {
            ui.EndResolutionPhase();
            Message.Publish(new ResolutionsFinished());
        }
    }

    protected override void Execute(ApplyBattleEffect msg)
    {
        var battleSnapshotBefore = state.GetSnapshot();
        AllEffects.Apply(msg.Effect, msg.Source, msg.Target);
        var battleSnapshotAfter = state.GetSnapshot();
        var effectResolved = new EffectResolved(msg.Effect, msg.Source, msg.Target, battleSnapshotBefore, battleSnapshotAfter);

        var reactions = state.Members.Values.SelectMany(v => v.State.GetReactions(effectResolved));
        reactions.ForEach(r => _reactions.Enqueue(r));
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    protected override void Execute(CardResolutionFinished msg) => ResolveNext();

    private IEnumerator ResolveAllReactions()
    {
        while (_reactions.Any())
        {
            yield return new WaitForSeconds(delay);
            var r = _reactions.Dequeue();
            var cost = r.Reaction.Cost;
            // TODO: Implement Gains if applicable
            // TODO: Animate the resolution of this effect, showing card on screen
            if (r.Source.CanAfford(cost))
            {
                var expense = cost.ResourcesSpent(r.Source);
                r.Source.Apply(s => s.Lose(expense));
                r.Reaction.ActionSequence.Perform(r.Source, r.Target, expense.Amount);
            }
        }
    }
    
    private void CheckForUnconsciousMembers() 
        => state.Members.Values
            .Except(_unconscious)
            .Where(m => !m.State.IsConscious)
            .CopiedForEach(ResolveUnconsciousMember);

    private void ResolveUnconsciousMember(Member member)
    {
        resolutionZone.ExpirePlayedCards(card => card.Member.Id == member.Id);
        _unconscious.Add(member);
        if (member.TeamType == TeamType.Enemies)
            state.AddRewardCredits(state.GetEnemyById(member.Id).RewardCredits);
        Message.Publish(new MemberUnconscious(member));
    }
}
