using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Condition/RequiresSkill")]
[Obsolete]
public class RequiresSkill : StoryEventCondition
{
    [SerializeField] private StringVariable skillName;

    public override bool Evaluate(StoryEventContext ctx) => true;

    public override string ConditionDescription => $"[Requires Skill: {skillName.Value}]";
}
