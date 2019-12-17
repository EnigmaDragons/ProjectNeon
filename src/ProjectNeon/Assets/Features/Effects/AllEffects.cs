using System;
using System.Collections.Generic;
using UnityEngine;

public static class AllEffects
{
    private static readonly Dictionary<EffectType, Func<EffectData, Effect>> _createEffectOfType = new Dictionary<EffectType, Func<EffectData, Effect>>
    {
        { EffectType.Nothing, e => new NoEffect() },
        { EffectType.HealFlat, e => new Heal(e.IntAmount) },
        { EffectType.PhysicalDamage, e => new Damage(new PhysicalDamage(e.FloatAmount)) },
        { EffectType.BuffAttackFlat, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new BuffedStats(new StatAddends().With(StatType.Attack, e.IntAmount), e.NumberOfTurns)))},
        { EffectType.BuffAttackMultiplier, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(new BuffedStats(new StatMultipliers().With(StatType.Attack, e.FloatAmount), e.NumberOfTurns)))},
        { EffectType.RemoveDebuffs, e => new SimpleEffect(m => m.RemoveTemporaryEffects(effect => effect.IsDebuff))},
        { EffectType.BuffMaxHp, e => new SimpleEffect(m =>
            {
                m.ApplyAdditiveUntilEndOfBattle(new StatAddends().With(StatType.MaxHP, e.IntAmount));
                m.GainHp(e.IntAmount);
            })},
        { EffectType.ShieldFlat, e => new ShieldFlat(e.IntAmount) },
        { EffectType.ResourceFlat, e => new SimpleEffect(m => m.GainResource(e.EffectScope.Value, e.IntAmount))},
        { EffectType.DamageOverTimeFlat, e => new DamageOverTime(e) },
        { EffectType.ApplyVulnerable, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(new DebuffedStats(new StatMultipliers().With(StatType.Damagability, 1.33f), e.NumberOfTurns))) },
        { EffectType.ShieldToughness, e => new SimpleEffect((src, m) => m.GainShield(e.IntAmount * src.State.Toughness())) },
        { EffectType.ArmorFlat, e => new SimpleEffect(m => m.GainArmor(e.IntAmount)) },
        { EffectType.Stun, e => new SimpleEffect(target => target.Stun(e.NumberOfTurns)) },
        { EffectType.ShieldAttackedOnAttack, e => new EffectOnAttacked(new SimpleEffect((src, m) => m.GainShield(e.IntAmount * src.State.Toughness()))) },
        { EffectType.DamageAttackerOnAttack, e => new EffectOnAttacker(new Damage(new PhysicalDamage(e.IntAmount))) },
        { EffectType.StealLifeNextAttack, e => new Recurrent(new StealLife(e.FloatAmount), 1)},
        { EffectType.InterceptAttackForTurns, e => new ForNumberOfTurns(new InterceptAttack(), e.NumberOfTurns)},
        { EffectType.Attack, e => new Attack(e.FloatAmount)},
        { EffectType.EvadeAttacks, e => new Recurrent(new Evade(), e.IntAmount) },
        { EffectType.HealFlatForTurnsOnTurnStart, e => new HealFlatForTurnsOnTurnStart(e.IntAmount, e.NumberOfTurns) },
        { EffectType.BuffStrengthFlat, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new BuffedStats(new StatAddends().With(StatType.Attack, e.IntAmount), e.NumberOfTurns)))},
        { EffectType.BuffToughnessFlat, e => new SimpleEffect(m => m.ApplyTemporaryAdditive(new BuffedStats(new StatAddends().With(StatType.Toughness, e.IntAmount), e.NumberOfTurns)))},
        { EffectType.RepeatEffect, e => new RepeatEffect(Create(e.origin), e.IntAmount) },
        { EffectType.RandomizeTarget, e => new RandomizeTarget(Create(e.origin)) },
        { EffectType.ExcludeSelfFromEffect, e => new ExcludeSelfFromEffect(Create(e.origin)) },
        { EffectType.ShieldBasedOnShieldValue, e => new SimpleEffect((src, m) => m.GainShield(e.FloatAmount * src.State[TemporalStatType.Shield])) },
        { EffectType.ForNumberOfTurns, e => new ForNumberOfTurns(Create(e.origin), e.IntAmount) },
        { EffectType.OnAttacked, e => new EffectOnAttacked(Create(e.origin)) },
        { EffectType.CostPrimaryResourceEffect, e => new CostPrimaryResourceEffect(Create(e.origin), e.IntAmount) },
        { EffectType.AnyTargetHealthBelowThreshold, e => new AnyTargetHealthBelowThreshold(Create(e.origin), e.FloatAmount) },
        { EffectType.SpellFlatDamageEffect, e => new SpellFlatDamageEffect(e.IntAmount) },
        { EffectType.RepeatUntilPrimaryResourceDepleted, e => new RepeatUntilPrimaryResourceDepleted(Create(e.origin), e.IntAmount) },
        { EffectType.OnNextTurnEffect, e => new OnNextTurnEffect(Create(e.origin)) },
        { EffectType.QueueEffect, e => new QueueEffect(Create(e.origin)) },
        { EffectType.EffectOnTurnStart, e => new EffectOnTurnStart(Create(e.origin)) },
        { EffectType.TriggerFeedEffects, e => new TriggerFeedEffects(Create(e.origin), e.EffectScope) },
        { EffectType.SetFeedUpEffect, e => new SetFeedUpEffect(Create(e.origin), e.EffectScope) },
        { EffectType.HealPrimaryResource, e => new SimpleEffect((src, m) => m.GainHp(src.State.PrimaryResourceAmount)) },
        { EffectType.BuffMagicMultiplier, e => new SimpleEffect(m => m.ApplyTemporaryMultiplier(new BuffedStats(new StatMultipliers().With(StatType.Magic, e.FloatAmount), e.NumberOfTurns)))},

    };
    /**
     * @todo #361:30min We sdhould be able to chain effects conditionally, as in MarkOfSalvation paladin card.
     */
    
    public static void Apply(EffectData effectData, Member source, Target target)
    {
        Create(effectData).Apply(source, target);
    }

    public static Effect Create(EffectData effectData)
    {
        var effectType = effectData.EffectType;
        if (!_createEffectOfType.ContainsKey(effectData.EffectType))
        {
            Debug.LogError($"No EffectType of {effectData.EffectType} exists in {nameof(AllEffects)}");
            return _createEffectOfType[EffectType.Nothing](effectData);
        }
        return _createEffectOfType[effectType](effectData);
    }
}
