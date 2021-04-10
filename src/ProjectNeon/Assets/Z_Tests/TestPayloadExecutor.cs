
using System.Linq;

public static class TestPayloadExecutor
{
    public static void ExecuteStartOfTurnEffects(this Member m)
        => m.State.GetTurnStartEffects()
            .ForEach(e => e.ExecuteAll(EffectContext.ForTests(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, new UnpreventableContext())));
    
    public static void ExecuteAll(this IPayloadProvider p, EffectContext ctx)
    {
        while (!p.IsFinished())
        {
            var next = p.GetNext();
            if (next.Payload is ApplyBattleEffect battleEffect)
                AllEffects.Apply(battleEffect.Effect, ctx);
            if (next.Payload is SinglePayload current)
                ExecuteAll(current, ctx);
        }
        ctx.Preventions.UpdatePreventionCounters();
    }

    public static void Execute(this Card card, Target[] targets, ResourceQuantity xPaidAmount, params Member[] allBattleMembers)
    {
        var payloads = card.GetPayloads(targets, new BattleStateSnapshot(allBattleMembers.Select(m => m.GetSnapshot()).ToArray()), xPaidAmount);
        for (var i = 0; i < payloads.Count; i++)
            ExecuteAll(payloads[i], EffectContext.ForTests(card.Owner, targets[i], card, xPaidAmount, new PreventionContextMut(targets[i])));
    }
}
