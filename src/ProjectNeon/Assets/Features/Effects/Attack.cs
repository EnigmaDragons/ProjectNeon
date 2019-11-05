


using UnityEngine;

public sealed class Attack  : Effect
{

    public Member Attacker { get; private set; }
    public Target Target { get; private set; }
    public int Damage { get; }

    public Attack(int damage)
    {
        Damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        Attacker = source;
        Target = target;
        if (target.Members.Length > 1)
        {
            Target.ApplyToAll((damage, source, target) => );
            target.Members.ForEach(
                member => {
                    new Attack(Damage).Apply(source, target);
                }
            );
        } else
        {
            new PhysicalDamage(Damage).Apply(source, target);
            BattleEvent.Publish(
                new AttackPerformed(this, source, target)
            );
        }
    }

    public void Apply(Member source, Member target) {
        Apply(source, new MemberAsTarget(target));
    }
}
