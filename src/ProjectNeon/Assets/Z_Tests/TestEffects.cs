using System.Linq;

public static class TestEffects
{
    public static EffectData BasicAttack => new EffectData
    {
        EffectType = EffectType.AttackFormula,
        Formula = "1 * Attack"
    };
    
    public static CardActionsData EmptyCardActionsData() =>
        TestableObjectFactory.Create<CardActionsData>()
            .Initialized(new CardActionV2(new EffectData {EffectType = EffectType.Nothing}));

    public static void ApplyBasicAttack(Member source, Member target)
        => Apply(BasicAttack, source, target);
    
    public static void Apply(EffectData effectData, Member source, Member target)
        => ApplyEffectAndReactions(effectData, source, target, Maybe<Card>.Missing());
    
    public static void Apply(EffectData effectData, Member source, Target target)
        => ApplyEffectAndReactions(effectData, source, target, Maybe<Card>.Missing(), ResourceQuantity.None);
    
    public static void Apply(EffectData effectData, Member source, Member target, Maybe<Card> card)
        => ApplyEffectAndReactions(effectData, source, new Single(target), card, ResourceQuantity.None);

    public static void Apply(EffectData effectData, Member source, Target target, Maybe<Card> card)
        => ApplyEffectAndReactions(effectData, source, target, card, ResourceQuantity.None);
    
    public static void Apply(EffectData effectData, Member source, Member target, Maybe<Card> card, ResourceQuantity xAmountPaid)
        => ApplyEffectAndReactions(effectData, source, new Single(target), card, xAmountPaid);

    public static void Apply(EffectData effectData, Member source, Target target, Maybe<Card> card, ResourceQuantity xAmountPaid)
        => ApplyEffectAndReactions(effectData, source, target, card, xAmountPaid);

    private static void ApplyEffectAndReactions(EffectData e, Member source, Member target, Maybe<Card> card)
        => ApplyEffectAndReactions(e, source, new Single(target), card, ResourceQuantity.None);

    private static void ApplyEffectAndReactions(EffectData e, Member source, Target target, Maybe<Card> card, ResourceQuantity xAmountPaid)
    {
        var tempMembers = target.Members.ToDictionary(t => t.Id, t => t);
        tempMembers[source.Id] = source;
        var members = tempMembers.Values;
        var battleSnapshotBefore = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());
        AllEffects.Apply(e, EffectContext.ForTests(source, target, card, xAmountPaid));
        var battleSnapshotAfter = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());

        var effectResolved = new EffectResolved(e, source, target, battleSnapshotBefore, battleSnapshotAfter, isReaction: false);

        var reactions = members.SelectMany(x => x.State.GetReactions(effectResolved));

        reactions.ForEach(r => r.ReactionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, EffectContext.ForTests(r.Source, r.Target, Maybe<Card>.Missing(), ResourceQuantity.None))));
    }
}
