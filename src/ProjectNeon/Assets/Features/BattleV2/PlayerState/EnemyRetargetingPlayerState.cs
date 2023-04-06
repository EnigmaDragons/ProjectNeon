using System.Linq;
//Super custom for a boss, can be generified if need be in the future
public class EnemyRetargetingPlayerState : TemporalStateBase, ITemporalPlayerState
{
    public IPlayerStats PlayerStats { get; } = new PlayerStatAddends();

    public override ITemporalState CloneOriginal() => new EnemyRetargetingPlayerState(OriginatorId, Tracker.Metadata.MaxDurationTurns);
    
    public override IStats Stats => new StatAddends();
    public override Maybe<int> Amount { get; } = Maybe<int>.Missing();

    public override IPayloadProvider OnTurnStart() => new NoPayload();
    public override IPayloadProvider OnTurnEnd() => new NoPayload();

    void ITemporalPlayerState.OnTurnStart() {}
    void ITemporalPlayerState.OnTurnEnd() => Tracker.AdvanceTurn();

    //should be non-deterministic so players can't save scum the retargeting system
    public IPlayedCard PossiblyRetargeted(BattleState state, IPlayedCard card)
    {
        var shouldRetarget = Rng.Bool();
        if (!shouldRetarget)
            return card;
        return card.Retargeted(card.Targets.Select((x, i) => Retarget(state, card.Member, card.Card.ActionSequences[i].Group, card.Card.ActionSequences[i].Scope, x)).ToArray());
    }

    private Target Retarget(BattleState state, Member source, Group group, Scope scope, Target target)
    {
        if (!state.Members[OriginatorId].IsConscious()
         || source.TeamType == TeamType.Enemies
         || group == Group.Self
         || (group == Group.All 
            && (scope == Scope.All 
             || scope == Scope.Random 
             || scope == Scope.AllExceptSelf 
             || scope == Scope.AllExcept)))
            return target;

        if (target.Members.Any(x => x.Id == OriginatorId))
        {
            var memberToSwapTo = state.GetConsciousAllies(source).Where(hero => hero.Id != source.Id && target.Members.None(member => member.Id == hero.Id)).ToArray().Shuffled().FirstAsMaybe();
            if (memberToSwapTo.IsMissing)
                return target;
            Message.Publish(new EnemyRetargetedPlayerCard(OriginatorId, memberToSwapTo.Value.Id));
            return new Multiple(target.Members.Select(x => x.Id == OriginatorId ? memberToSwapTo.Value : x));
        }
        else
        {
            var memberToSwapFrom = target.Members.Where(x => x.TeamType == TeamType.Party).Where(hero => hero.Id != source.Id).ToArray().Shuffled().FirstAsMaybe();
            if (memberToSwapFrom.IsMissing)
                return target;
            Message.Publish(new EnemyRetargetedPlayerCard(OriginatorId, memberToSwapFrom.Value.Id));
            return new Multiple(target.Members.Select(x => x.Id == memberToSwapFrom.Value.Id ? state.Members[OriginatorId] : x));
        }
    }

    public EnemyRetargetingPlayerState(int originatingId, int duration) : base(TemporalStateMetadata.ForDuration(originatingId, duration, true)) { }
}