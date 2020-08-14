public static class TestCards
{
    public static ReactionCardType Reaction(ReactiveMember src, ReactiveTargetScope target, EffectData e)
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
}
