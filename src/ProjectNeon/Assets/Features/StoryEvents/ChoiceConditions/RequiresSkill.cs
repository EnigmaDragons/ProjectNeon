using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Condition/RequiresTreatmentSkill")]
public class RequiresSkill : StoryEventCondition
{
    [SerializeField] private StringReference skillName;

    public override bool Evaluate(StoryEventContext ctx) =>
        ctx.Party
            .Heroes.Any(h => h
                .Skills.Any(s => s.SkillName.Equals(skillName, StringComparison.InvariantCultureIgnoreCase)));
}
