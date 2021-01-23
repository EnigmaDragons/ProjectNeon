using System.Collections.Generic;
using System.Linq;

public static class TestEffects
{
    public static void ApplyEffectAndReactions(EffectData e, Member source, Member target)
        => ApplyEffectAndReactions(e, source, new Single(target));
    
    public static void ApplyEffectAndReactions(EffectData e, Member source, Target target)
    {
        var tempMembers = target.Members.ToDictionary(t => t.Id, t => t);
        tempMembers[source.Id] = source;
        var members = tempMembers.Values;
        var battleSnapshotBefore = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());
        AllEffects.Apply(e, new EffectContext(source, target));
        var battleSnapshotAfter = new BattleStateSnapshot(members.Select(m => m.GetSnapshot()).ToArray());

        var effectResolved = new EffectResolved(e, source, target, battleSnapshotBefore, battleSnapshotAfter, isReaction: false);

        var reactions = members.SelectMany(x => x.State.GetReactions(effectResolved));

        reactions.ForEach(r => r.Reaction.ActionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, new EffectContext(r.Source, r.Target))));
    }

    public static void Apply(EffectData effectData, Member source, Member target)
        => ApplyEffectAndReactions(effectData, source, target);
    
    public static void Apply(EffectData effectData, Member source, Target target)
        => ApplyEffectAndReactions(effectData, source, target);

    public static CardActionsData EmptyCardActionsData() =>
        TestableObjectFactory.Create<CardActionsData>()
            .Initialized(new CardActionV2(new EffectData {EffectType = EffectType.Nothing}));
}
