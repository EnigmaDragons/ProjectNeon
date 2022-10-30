﻿using System.Linq;
using UnityEngine;

public sealed class DealDamage : Effect
{
    private readonly DamageCalculation _damage;

    public DealDamage(DamageCalculation damage) => _damage = damage;

    public void Apply(EffectContext ctx)
    {
        ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Dodge, ctx.Target.Members);
        ctx.Target.Members.ForEach(m =>
        {
            var amount = Mathf.CeilToInt(_damage.Calculate(ctx, m) * m.State[StatType.Damagability]);
            var wasVulnerableString = m.IsVulnerable() ? " (Vulnerable) " : " ";
            if (ctx.Preventions.IsDodging(m))
            {
                BattleLog.Write($"{m.UnambiguousEnglishName} Dodged the attack.");
                return;
            }
            
            if (m.State[StatType.Damagability] < 0.01)
                BattleLog.Write($"0 damage was dealt to Invincible {m.UnambiguousEnglishName}");
            else
            {
                BattleLog.Write($"{amount} damage dealt to{wasVulnerableString}{m.UnambiguousEnglishName}");
                if (_damage.DealTrueDamage)
                    m.State.TakeTrueDamage(amount);
                else
                    m.State.TakeDamage(amount);
                if (amount < 1)
                    Message.Publish(new DisplayCharacterWordRequested(m.Id, CharacterReactionType.TookZeroDamage));
            }
        });
    }
}
