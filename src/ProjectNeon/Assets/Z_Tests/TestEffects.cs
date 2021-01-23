using System.Linq;

public static class TestEffects
{
    public static CardActionsData EmptyCardActionsData() =>
        TestableObjectFactory.Create<CardActionsData>()
            .Initialized(new CardActionV2(new EffectData {EffectType = EffectType.Nothing}));
    
    public static void Apply(EffectData effectData, Member source, Member target)
        => ApplyEffectAndReactions(effectData, source, target);
    
    public static void Apply(EffectData effectData, Member source, Target target)
        => ApplyEffectAndReactions(effectData, source, target);

    private static void ApplyEffectAndReactions(EffectData e, Member source, Member target)
        => ApplyEffectAndReactions(e, source, new Single(target));

    private static void ApplyEffectAndReactions(EffectData e, Member source, Target target)
    {
        var tempMembers = target.Members.ToDictionary(t => t.Id, t => t);
        tempMembers[source.Id] = source;
        var members = tempMembers.Values;
        var battleSnapshotBefore = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());
        AllEffects.Apply(e, new EffectContext(source, target));
        var battleSnapshotAfter = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());

        var effectResolved = new EffectResolved(e, source, target, battleSnapshotBefore, battleSnapshotAfter, isReaction: false);

        var reactions = members.SelectMany(x => x.State.GetReactions(effectResolved));

        reactions.ForEach(r => r.ReactionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, new EffectContext(r.Source, r.Target))));
    }
}
