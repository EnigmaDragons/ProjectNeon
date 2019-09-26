using UnityEngine;

public class Heal : Effect
{
    [SerializeField] private int quantity;

    public override void Apply(Target target)
    {
        target.Members.ForEach(member => member.hp += this.quantity);
    }
}
