using UnityEngine;

public class BattleResolutionPhase : OnMessage<ApplyBattleEffect, CardResolutionFinished>
{
    [SerializeField] private CardResolutionZone resolutionZone;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);
    
    public void Begin()
    {
        BattleLog.Write($"Card Resolution Began");
        ResolveNext();
    }

    private void ResolveNext()
    {
        if (resolutionZone.HasMore)
            StartCoroutine(resolutionZone.ResolveNext(delay));
        else
            Message.Publish(new ResolutionsFinished());
    }

    protected override void Execute(ApplyBattleEffect msg)
    {
        AllEffects.Apply(msg.Effect, msg.Source, msg.Target);
        Message.Publish(new Finished<ApplyBattleEffect>());
    }

    protected override void Execute(CardResolutionFinished msg)
    {
        ResolveNext();
    }
}
