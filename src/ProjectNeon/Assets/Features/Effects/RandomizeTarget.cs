using UnityEngine;
using UnityEditor;

/**
 * Apply the efect on a random target among the original targets.
 */
public class RandomizeTarget : Effect
{
    private Effect _effect;

    public RandomizeTarget(Effect effect)
    {
        _effect = effect;
    }

    public void Apply(Member source, Target target)
    {
        _effect.Apply(source, new Single(target.Members[Random.Range(0, target.Members.Length)]));
    }
}