using System;
using System.Collections.Generic;
using System.Linq;
using Features.CombatantStates.Reactions;

public class EffectReactWithEffect : Effect
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
    private readonly StatusDetail _status;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly CardReactionSequence _reaction;
    private readonly ReactionConditionType _conditionType;
    private readonly Func<Member, Func<EffectResolved, bool>> _conditionBuilder;

    public EffectReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, 
        ReactiveTriggerScope triggerScope, CardReactionSequence reaction, ReactionConditionType conditionType)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _status = status;
        _triggerScope = triggerScope;
        _reaction = reaction;
        _conditionType = conditionType;
        _conditionBuilder = Conditions.VerboseGetValue(conditionType, conditionType.ToString());
    }
    
    public void Apply(EffectContext ctx)
    {
        ctx.Target.ApplyToAllConscious(m =>
        {
            m.AddReactiveState(new ReactWithEffect(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope,
                ctx.BattleMembers, m.MemberId, ctx.Source, _reaction,
                _conditionBuilder(ctx.BattleMembers[m.MemberId])));
            DevLog.Write($"Applied React {_conditionType} to {m.Name}");
        });
    }
}

public sealed class ReactWithEffect : ReactiveEffectV2Base
{
    public ReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, CardReactionSequence reaction, Func<EffectResolved, bool> condition)
            : base(isDebuff, maxDurationTurns, numberOfUses, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
                effect =>
                {
                    var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                    var conditionMet = condition(effect);
                    DevLog.Write($"Reaction - Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                    return isInTriggerScope && conditionMet;
                }))
    {
        Status = status;
    }

    public override StatusDetail Status { get; }
}
