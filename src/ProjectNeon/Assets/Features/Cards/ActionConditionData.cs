using System;

[Serializable]
public class ActionConditionData
{
    public ActionConditionType ConditionType;
    public ResourceCost Cost = new ResourceCost();
    public CardActionsData ReferencedEffect;
}