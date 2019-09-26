using UnityEngine;

/**
 * Applies Damage to some Target, i.e., reduces the Target H.P. in the quantity value.
 */
public class Damage : Effect
{
    [SerializeField] private int quantity;
    public override void Apply(Target target)
    {
        target.Members.ForEach(member => member.hp -= this.quantity);
    }
}
