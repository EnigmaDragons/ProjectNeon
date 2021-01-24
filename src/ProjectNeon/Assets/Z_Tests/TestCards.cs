public static class TestCards
{
    public static ReactionCardType AnyReaction() =>
        ReactionCard(ReactiveMember.Originator, ReactiveTargetScope.Source, EffectData.Nothing);

    public static ReactionCardType ReactionOnAttacked(EffectData e)
        => ReactionCard(ReactiveMember.Originator, ReactiveTargetScope.Source, e);
    
    public static ReactionCardType ReactionCard(ReactiveMember src, ReactiveTargetScope target, EffectData e)
    {
        var resourceType = TestableObjectFactory.Create<TestResourceType>().Initialized("Ammo");
        var reactionCardType = TestableObjectFactory.Create<ReactionCardType>()
            .Initialized(
                new ResourceCost(0, resourceType),
                new ResourceCost(0, resourceType),
                new CardReactionSequence(src, target,
                    TestableObjectFactory.Create<CardActionsData>()
                        .Initialized(new CardActionV2(e))));
        return reactionCardType;
    }
    
    public static CardReactionSequence ReactionEffect(ReactiveMember src, ReactiveTargetScope target, EffectData e) =>
        new CardReactionSequence(src, target,
            TestableObjectFactory.Create<CardActionsData>()
                .Initialized(new CardActionV2(e)));
}
