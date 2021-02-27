
public static class TestPayloadExecutor
{
    public static void ExecuteStartOfTurnEffects(this Member m)
        => m.State.GetTurnStartEffects()
            .ForEach(e => e.ExecuteAll(EffectContext.ForTests(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None)));
    
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
    }
}
