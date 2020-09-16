
public static class TestEffects
{
    public static void Apply(EffectData effectData, Member source, Member target)
        => AllEffects.Apply(effectData, new EffectContext(source, new Single(target)));

    public static void Apply(EffectData effectData, Member source, Target target)
        => AllEffects.Apply(effectData, new EffectContext(source, target));

    public static CardActionsData EmptyCardActionsData() =>
        TestableObjectFactory.Create<CardActionsData>()
            .Initialized(new CardActionV2(new EffectData {EffectType = EffectType.Nothing}));
}
