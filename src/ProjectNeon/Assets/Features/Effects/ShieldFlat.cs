using UnityEngine;
using UnityEditor;

public sealed class ShieldFlat : Effect
{
    private int _amount;

    public ShieldFlat(int amount) {
        _amount = amount;
    }

    public void Apply(Member source, Target target)
    {
        target.Members.ForEach(
            member => member.State.GainShield(_amount)
        );
    }
}