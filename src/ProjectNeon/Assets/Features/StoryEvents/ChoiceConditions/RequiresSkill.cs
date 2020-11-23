using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Condition/RequiresSkill")]
public class RequiresSkill : StoryEventCondition
{
    [SerializeField] private StringVariable skillName;

    public override bool Evaluate(StoryEventContext ctx) =>
        ctx.Party
            .Heroes.Any(h => h
                .Skills.Any(s => s.SkillName.Value.Equals(skillName.Value, StringComparison.InvariantCultureIgnoreCase)));

    public override string ConditionDescription => $"[Requires Skill: {skillName.Value}]";
}
