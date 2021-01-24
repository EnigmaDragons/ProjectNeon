using System;
using System.Collections.Generic;
using System.Linq;
using Features.CombatantStates.Reactions;

public class EffectReactWith : Effect
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
        },
        { ReactionConditionType.OnAttacked, possessor => effect => 
            effect.EffectData.EffectType == EffectType.Attack && effect.Target.Members.Any(x => x.Id == possessor.Id) },
        { ReactionConditionType.OnBloodied, possessor => effect => 
            !effect.BattleBefore.Members[possessor.Id].IsBloodied() && effect.BattleAfter.Members[possessor.Id].IsBloodied() },
        { ReactionConditionType.OnShieldBroken, possessor => effect => 
            effect.BattleBefore.Members[possessor.Id].State.Counters["Shield"] > 0 
                && effect.BattleAfter.Members[possessor.Id].State.Counters["Shield"] == 0 },
        { ReactionConditionType.OnDamaged, possessor => effect => 
            effect.BattleBefore.Members[possessor.Id].State.Hp > effect.BattleAfter.Members[possessor.Id].State.Hp},
        { ReactionConditionType.OnCausedHeal, possessor => effect =>
            {
                if (!Equals(possessor, effect.Source))
                    return false;
                
                var targetMembers = effect.Target.Members;
                var hpBefore = targetMembers.Select(t => effect.BattleBefore.Members[t.Id])
                    .Sum(x => x.State.Hp);
                var hpAfter = targetMembers.Select(t => effect.BattleAfter.Members[t.Id])
                    .Sum(x => x.State.Hp);
                return hpAfter > hpBefore;
            }
        },
        {ReactionConditionType.OnVulnerable, possessor => effect 
            => (effect.EffectData.EffectType == EffectType.ApplyVulnerable || effect.EffectData.EffectScope.Value == "Vulnerable") && effect.Target.Members.Any(x => x.Id == possessor.Id) }
    };
    
    private readonly bool _isDebuff;
    private readonly int _numberOfUses;
    private readonly int _maxDurationTurns;
    private readonly StatusDetail _status;
    private readonly ReactiveTriggerScope _triggerScope;
    private readonly Maybe<CardReactionSequence> _reactionEffect;
    private readonly Maybe<ReactionCardType> _reactionCard;
    private readonly ReactionConditionType _conditionType;
    private readonly Func<Member, Func<EffectResolved, bool>> _conditionBuilder;

    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status,
        ReactiveTriggerScope triggerScope, ReactionConditionType conditionType, ReactionCardType reactionCard)
            : this(isDebuff, numberOfUses, maxDurationTurns, status, triggerScope, conditionType, Maybe<CardReactionSequence>.Missing(), 
                new Maybe<ReactionCardType>(reactionCard, true)) {}
    
    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status,
        ReactiveTriggerScope triggerScope, ReactionConditionType conditionType, CardReactionSequence reactionEffect)
        : this(isDebuff, numberOfUses, maxDurationTurns, status, triggerScope, conditionType, new Maybe<CardReactionSequence>(reactionEffect, true), 
            Maybe<ReactionCardType>.Missing()) {}
        
    public EffectReactWith(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        ReactionConditionType conditionType, Maybe<CardReactionSequence> reactionEffect, Maybe<ReactionCardType> reactionCard)
    {
        _isDebuff = isDebuff;
        _numberOfUses = numberOfUses;
        _maxDurationTurns = maxDurationTurns;
        _status = status;
        _triggerScope = triggerScope;
        _reactionEffect = reactionEffect;
        _reactionCard = reactionCard;
        _conditionType = conditionType;
        _conditionBuilder = Conditions.VerboseGetValue(conditionType, conditionType.ToString());
        if (reactionCard.IsMissing && reactionEffect.IsMissing)
            Log.Info($"React With neither has an Effect nor a Card.");
    }
    
    public void Apply(EffectContext ctx)
    {
        if (_reactionCard.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                m.AddReactiveState(new ReactWithCard(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope,
                    ctx.BattleMembers, m.MemberId, ctx.Source, _reactionCard.Value,
                    _conditionBuilder(ctx.BattleMembers[m.MemberId])));
                DevLog.Write($"Applied React With Card {_conditionType} to {m.Name}");
            });
        else if (_reactionEffect.IsPresent)
            ctx.Target.ApplyToAllConscious(m =>
            {
                m.AddReactiveState(new ReactWithEffect(_isDebuff, _numberOfUses, _maxDurationTurns, _status, _triggerScope,
                    ctx.BattleMembers, m.MemberId, ctx.Source, _reactionEffect.Value,
                    _conditionBuilder(ctx.BattleMembers[m.MemberId])));
                DevLog.Write($"Applied React With Effect {_conditionType} to {m.Name}");
            });
    }
}

public sealed class ReactWithEffect : ReactiveEffectV2Base
{
    public ReactWithEffect(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, CardReactionSequence reaction, Func<EffectResolved, bool> condition)
            : base(isDebuff, maxDurationTurns, numberOfUses, status, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
                effect =>
                {
                    var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                    var conditionMet = condition(effect);
                    DevLog.Write($"{status.Tag} Reaction - Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                    return isInTriggerScope && conditionMet;
                })) {}
}

public sealed class ReactWithCard : ReactiveEffectV2Base
{
    public ReactWithCard(bool isDebuff, int numberOfUses, int maxDurationTurns, StatusDetail status, ReactiveTriggerScope triggerScope, 
        IDictionary<int, Member> allMembers, int possessingMemberId, Member originator, ReactionCardType reaction, Func<EffectResolved, bool> condition)
        : base(isDebuff, maxDurationTurns, numberOfUses, status, CreateMaybeEffect(allMembers, possessingMemberId, originator, false, reaction, 
            effect =>
            {
                var isInTriggerScope = triggerScope.IsInTriggerScope(originator, allMembers[possessingMemberId], effect.Source);
                var conditionMet = condition(effect);
                DevLog.Write($"{status.Tag} Reaction - Is In Trigger Scope: {isInTriggerScope}. Condition Met: {conditionMet}");
                return isInTriggerScope && conditionMet;
            })) {}
}

