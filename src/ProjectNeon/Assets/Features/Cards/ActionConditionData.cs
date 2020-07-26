using System;
using UnityEngine;

[Serializable]
public class ActionConditionData
{
    public ActionConditionType ConditionType;
    public FloatReference FloatAmount = new FloatReference();
    public int IntAmount => Mathf.CeilToInt(FloatAmount.Value);
    public StringReference EffectScope = new StringReference();
    public CardActionsData ReferencedEffect;
}