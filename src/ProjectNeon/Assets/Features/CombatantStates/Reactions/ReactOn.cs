using System;
using System.Collections.Generic;
using System.Linq;
using Features.CombatantStates.Reactions;

public class EffectReactOn : Effect
{
    private static Dictionary<ReactionConditionType, Func<Member, Func<EffectResolved, bool>>> Conditions = new Dictionary<ReactionConditionType, Func<Member, Func<EffectResolved, bool>>>
    {
        { ReactionConditionType.OnCausedStun, possessor => effect =>
            {
                if (!Equals(possessor, effect.Source))
                    return false;
                
                var targetMembers = effect.Target.Members;
                var stunsBefore = targetMembers.Select(t => effect.BattleBefore.Members[t.Id])
                    .Sum(x => x.State.Counter(TemporalStatType.CardStun) + x.State.Counter(TemporalStatType.TurnStun));
                var stunsAfter = targetMembers.Select(t => effect.BattleAfter.Members[t.Id])
                    .Sum(x => x.State.Counter(TemporalStatType.CardStun) + x.State.Counter(TemporalStatType.TurnStun));
                return stunsAfter > stunsBefore;
            }
        }
    };
    
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly StatusTag _tag;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly ReactionCardType _reaction;
    private readonly Func<Member, Func<EffectResolved, bool>> _conditionBuilder;

    public EffectReactOn(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusTag tag, 
        ReactiveTriggerScope triggerScope, ReactionCardType reaction, ReactionConditionType conditionType)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _tag = tag;
        _triggerScope = triggerScope;
        _reaction = reaction;
        _conditionBuilder = Conditions.VerboseGetValue(conditionType, conditionType.ToString());
    }
    
    public void Apply(EffectContext ctx) =>
        ctx.Target.ApplyToAllConscious(m => 
            m.AddReactiveState(new ReactOn(_isDebuff, _numberOfUses, _maxDurationTurns, _tag, _triggerScope, ctx.BattleMembers, m.MemberId, ctx.Source, _reaction, 
                _conditionBuilder(ctx.BattleMembers[m.MemberId]))));
}

public sealed class ReactOn : ReactiveEffectV2Base
{
    public ReactOn(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusTag tag, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction,  Func<EffectResolved, bool> condition)
            : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
                effect => triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source) 
                          && condition(effect)))
    {
        Tag = tag;
    }

    public override StatusTag Tag { get; }
}
