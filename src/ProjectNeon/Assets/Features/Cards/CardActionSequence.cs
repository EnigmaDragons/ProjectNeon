using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardActionSequence
{
    [SerializeField] private Scope scope;
    [SerializeField] private Group group;
    [SerializeField] private AvoidanceType avoidance;
    [SerializeField] public CardActionsData cardActions;
    [SerializeField] private bool repeatX;

    public Scope Scope => scope;
    public Group Group => group;
    public AvoidanceType AvoidanceType => avoidance;
    public CardActionsData CardActions => cardActions;
    public bool RepeatX => repeatX;
    
    private static HashSet<EffectType> DamageEffectTypes = new HashSet<EffectType>
    {
        EffectType.MagicAttack,
        EffectType.MagicAttackFormula,
        EffectType.AttackFormula,
        EffectType.DealRawDamageFormula,
    };
    public bool HasDamageEffects => CardActions.BattleEffects.Any(e => DamageEffectTypes.Contains(e.EffectType));

    public static CardActionSequence Create(Scope s, Group g, AvoidanceType a, CardActionsData data, bool repeatX)
    {
        return new CardActionSequence
        {
            scope = s,
            @group = g,
            avoidance = a,
            cardActions = data,
            repeatX = repeatX
        };
    }
    
    public static CardActionSequence ForReaction(CardActionsData d)
    {
        var c = new CardActionSequence {cardActions = d};
        return c;
    }
}
