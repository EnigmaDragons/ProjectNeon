using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public abstract void Apply(Member source, Target target);
}
