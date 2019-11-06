
using UnityEngine;

public sealed class Attack  : Effect
{

    public Member Attacker { get; set; }
    public Target Target { get; set; }
    public int Damage { get; set; }
    public Effect Effect { get; set; }

    public Attack(int damage)
    {
        Damage = damage;
        Effect = new PhysicalDamage(Damage);
    }

    public void Apply(Member source, Target target)
    {
        Attacker = source;
        Target = target;
        if (target.Members.Length > 1)
        {
            target.Members.ForEach(
                member => {
                    new Attack(Damage).Apply(source, target);
                }
            );
        } else
        {
            BattleEvent.Publish(
                new AttackToPerform(this)
            );
            Effect.Apply(source, target);
            BattleEvent.Publish(
                new AttackPerformed(this)
            );
        }
    }

    public void Apply(Member source, Member target) {
        Apply(source, new MemberAsTarget(target));
    }
}
