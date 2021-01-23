
public static class TestEffects
{
    public static void Apply(EffectData effectData, Member source, Member target)
        => AllEffects.Apply(effectData, new EffectContext(source, new Single(target), Maybe<Card>.Missing()));

    public static void Apply(EffectData effectData, Member source, Target target)
        => AllEffects.Apply(effectData, new EffectContext(source, target, Maybe<Card>.Missing()));

    public static void Apply(EffectData effectData, Member source, Member target, Maybe<Card> card)
        => AllEffects.Apply(effectData, new EffectContext(source, new Single(target), card));

    public static void Apply(EffectData effectData, Member source, Target target, Maybe<Card> card)
        => AllEffects.Apply(effectData, new EffectContext(source, target, card));

    public static CardActionsData EmptyCardActionsData() =>
        TestableObjectFactory.Create<CardActionsData>()
            .Initialized(new CardActionV2(new EffectData {EffectType = EffectType.Nothing}));
}
