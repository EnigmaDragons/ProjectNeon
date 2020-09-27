using System.Linq;

public static class ReactiveTestUtilities
{
    public static void ApplyEffectAndReactions(EffectData e, Member source, Member target)
    {
        var battleSnapshotBefore = new BattleStateSnapshot(target.GetSnapshot());
        TestEffects.Apply(e, source, target);
        var battleSnapshotAfter = new BattleStateSnapshot(target.GetSnapshot());

        var effectResolved = new EffectResolved(e, source, new Single(target), battleSnapshotBefore, battleSnapshotAfter, isReaction: false);

        var reactions = target.State.GetReactions(effectResolved);
        reactions.ForEach(r => r.Reaction.ActionSequence.CardActions.Actions.Where(a => a.Type == CardBattleActionType.Battle)
            .ForEach(be => AllEffects.Apply(be.BattleEffect, new EffectContext(r.Source, r.Target))));
    }
}