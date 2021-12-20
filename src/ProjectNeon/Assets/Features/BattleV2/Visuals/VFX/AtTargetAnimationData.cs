using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class AtTargetAnimationData
{
    [SerializeField] private StringReference animation;
    [SerializeField] private float size = 1;
    [SerializeField] private float speed = 1;
    [SerializeField] private ColorReference color = new ColorReference(new Color(0, 0, 0, 0));
    [SerializeField] public StaticEffectCondition[] conditions;

    public string Animation => animation.Value;
    public float Size => size;
    public float Speed => speed;
    public Color Color => color.Value;
    public EffectCondition Condition 
        => conditions != null && conditions.Length > 0 
            ? (EffectCondition) new AndEffectCondition(conditions.Cast<EffectCondition>().ToArray()) 
            : new NoEffectCondition();
}