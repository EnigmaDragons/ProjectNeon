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
    
    public IEnumerator Begin()
    {
        BattleLog.Write($"Card Resolution Began");
        yield return ui.BeginResolutionPhase();
        ResolveNext();
    }

    private void ResolveNext()
    {
        CheckForUnconsciousMembers();
        if (resolutionZone.HasMore)
            StartCoroutine(resolutionZone.ResolveNext(delay));
        else
        {
            ui.EndResolutionPhase();
            Message.Publish(new ResolutionsFinished());
        }
    }

    protected override void Execute(ApplyBattleEffect msg)
    {
        AllEffects.Apply(msg.Effect, msg.Source, msg.Target);
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    protected override void Execute(CardResolutionFinished msg) => ResolveNext();

    private void CheckForUnconsciousMembers() 
        => state.Members.Values.ToList()
            .Except(_unconscious)
            .Where(m => !m.State.IsConscious)
            .CopiedForEach(ResolveUnconsciousMember);

    private void ResolveUnconsciousMember(Member member)
    {
        resolutionZone.ExpirePlayedCards(card => card.Member.Id == member.Id);
        _unconscious.Add(member);
        Message.Publish(new MemberUnconscious(member));
    }
}
