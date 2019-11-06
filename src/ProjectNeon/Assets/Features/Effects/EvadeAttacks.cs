using UnityEngine;
using UnityEditor;

public sealed class EvadeAttacks : Effect
{
    private Effect _effect;

    public EvadeAttacks(int quantity) 
    {
        _effect = new Recurrent(new Evade(), quantity);
    }

    public EvadeAttacks() : this(1)
    {

    }

    public void Apply(Member source, Target target)
    {
        _effect.Apply(source, target);
    }
}