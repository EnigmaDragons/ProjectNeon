using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Card effect
 */
public abstract class Effect : ScriptableObject
{

    public abstract void Apply(Target target);
}
