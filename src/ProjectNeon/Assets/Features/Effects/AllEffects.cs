using System;
using System.Collections.Generic;

public static class AllEffects
{
    private static IReadOnlyDictionary<int, Member> _members;
    private static PlayerState _playerState;
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> _createEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.HealFlat, e => new Heal(e.IntAmount) },
        { EffectType.PhysicalDamage, e => new Damage(new PhysicalDamage(e.FloatAmount)) },
        { EffectType.AdjustStatAdditively, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new AdjustedStats(new StatAddends().With((StatType)Enum.Parse(typeof(StatType), e.EffectScope, true), e.IntAmount), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns == -1)))},
        { EffectType.AdjustStatMultiplicatively, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(new AdjustedStats(new StatMultipliers().With((StatType)Enum.Parse(typeof(StatType), e.EffectScope, true), e.FloatAmount), e.NumberOfTurns, e.IntAmount < 0, e.IntAmount == -1)))},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => m.RemoveTemporaryEffects(effect => effect.IsDebuff))},
        { EffectType.ShieldFlat, e => new ShieldFlat(e.IntAmount) },
        { EffectType.ResourceFlat, e => new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.IntAmount))},
        { EffectType.DamageOverTimeFlat, e => new DamageOverTime(e) },
        { EffectType.ApplyVulnerable, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(new AdjustedStats(new StatMultipliers().With(StatType.Damagability, 1.33f), e.NumberOfTurns, true, e.NumberOfTurns == -1))) },
        { EffectType.ShieldToughness, e => new SimpleEffect((src, m) => m.GainShield(e.IntAmount * src.State.Toughness())) },
        { EffectType.RemoveShields, e => new SimpleEffect((src, m) => m.GainShield(-999)) },
        { EffectType.StunForTurns, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new StunForTurns(e.NumberOfTurns)))},
        { EffectType.StunForNumberOfCards, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(AdjustedStats.CreateIndefinite(new StatAddends().With(TemporalStatType.CardStun, e.IntAmount), true))) },
        { EffectType.StealLifeNextAttack, e => new NoEffect() }, // TODO: Implement Life Steal
        { EffectType.InterceptAttackForTurns, e => new InterceptAttack(e.NumberOfTurns)},
        { EffectType.Attack, e => new Attack(e.FloatAmount, e.HitsRandomTargetMember)},
        { EffectType.EvadeAttacks, e => new Evade(e.IntAmount) },
        { EffectType.HealOverTime, e => new HealOverTime(e.FloatAmount, e.NumberOfTurns) },
        { EffectType.RepeatEffect, e => new RepeatEffect(Create(e.origin), e.IntAmount) },
        { EffectType.RandomizeTarget, e => new RandomizeTarget(Create(e.origin)) },
        { EffectType.ExcludeSelfFromEffect, e => new ExcludeSelfFromEffect(Create(e.origin)) },
        { EffectType.ShieldBasedOnShieldValue, e => new SimpleEffect((src, m) => m.GainShield(e.FloatAmount * src.State[TemporalStatType.Shield])) },
        { EffectType.ForNumberOfTurns, e => new ForNumberOfTurns(Create(e.origin), e.IntAmount) },
        { EffectType.OnAttacked, e => new EffectOnAttacked(false, e.IntAmount, e.NumberOfTurns, e.ReactionSequence, _members) },
        { EffectType.OnEvaded, e => new EffectOnEvaded(false, e.IntAmount, e.NumberOfTurns, e.ReactionSequence, _members) },
        { EffectType.OnShieldBroken, e => new EffectOnShieldBroken(false, e.NumberOfTurns, e.ReactionSequence, _members) },
        { EffectType.CostResource, e => new SimpleEffect(m => m.Lose(new ResourceQuantity { Amount = e.IntAmount, ResourceType = e.EffectScope.Value }))},
        { EffectType.AnyTargetHealthBelowThreshold, e => new AnyTargetHealthBelowThreshold(Create(e.origin), e.FloatAmount) },
        { EffectType.SpellFlatDamageEffect, e => new SpellFlatDamageEffect(e.IntAmount) },
        { EffectType.RepeatUntilPrimaryResourceDepleted, e => new RepeatUntilPrimaryResourceDepleted(Create(e.origin), e.IntAmount) },
        { EffectType.OnNextTurnEffect, e => new OnNextTurnEffect(Create(e.origin)) },
        { EffectType.EffectOnTurnStart, e => new EffectOnTurnStart(Create(e.origin)) },
        { EffectType.TriggerFeedEffects, e => new TriggerFeedEffects(Create(e.origin), e.EffectScope) },
        { EffectType.ApplyOnShieldBelowValue, e => new ApplyOnShieldBelowValue(Create(e.origin), e.IntAmount) },
        { EffectType.ApplyOnChance, e => new ApplyOnChance(Create(e.origin), e.FloatAmount) },
        { EffectType.HealPrimaryResource, e => new SimpleEffect((src, m) => m.GainHp(src.State.PrimaryResourceAmount)) },
        { EffectType.ReplayLastCard, e => new ReplayLastCardEffect()},
        { EffectType.HealMagic, e => new HealMagic(e.FloatAmount) },
        { EffectType.GivePrimaryResource, e => new SimpleEffect(m => m.GainPrimaryResource(e.IntAmount)) },
        { EffectType.AdjustPlayerStats, e => new SimpleEffect(() => _playerState.AddState(new AdjustedPlayerStats(new PlayerStatAddends().With((PlayerStatType)Enum.Parse(typeof(PlayerStatType), e.EffectScope), e.IntAmount), e.NumberOfTurns, e.IntAmount < 0, e.NumberOfTurns < 0))) }
    };

    public static void Apply(EffectData effectData, Member source, Member target)
        => Apply(effectData, source, new Single(target));
    
    public static void Apply(EffectData effectData, Member source, Target target)
    {
        var effect = Create(effectData);
        BattleLog.Write($"Applying Effect of {effectData.EffectType} to {target.MembersDescriptions()}");
        effect.Apply(source, target);
    }

    public static Effect Create(EffectData effectData)
    {
        var effectType = effectData.EffectType;
        if (!_createEffectOfType.ContainsKey(effectData.EffectType))
        {
            Log.Error($"No EffectType of {effectData.EffectType} exists in {nameof(AllEffects)}");
            return _createEffectOfType[EffectType.Nothing](effectData);
        }
        return _createEffectOfType[effectType](effectData);
    }

    public static void InitTurnStart(IReadOnlyDictionary<int, Member> members, PlayerState playerState)
    {
        _members = members;
        _playerState = playerState;
    }
}
