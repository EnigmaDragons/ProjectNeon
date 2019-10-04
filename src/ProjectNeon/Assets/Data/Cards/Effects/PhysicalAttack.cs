using UnityEngine;

public class PhysicalAttack : Effect
{
    [SerializeField] private float multiplier = 1f;
    public override void Apply(Member member, Target target)
    {
        target.ApplyToAll(s => s.HP -= (int)(member.State.Attack * multiplier));
    }
}
