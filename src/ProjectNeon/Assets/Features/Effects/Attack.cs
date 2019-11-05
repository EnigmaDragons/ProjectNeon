


using UnityEngine;

public sealed class Attack  : Effect
{
    private Member _attacker;
    public Member Attacker() { return this._attacker; }
    private Target _target;
    public Target Target() { return this._target; }
    public int Damage { get; }

    public Attack(int damage)
    {
        Damage = damage;
    }

    public void Apply(Member source, Target target)
    {
        _attacker = source;
        _target = target;
        if (target.Members.Length > 1)
        {
            target.Members.ForEach(
                member => {
                    new Attack(Damage).Apply(source, target);
                }
            );
        } else
        {
            AllEffects.Create(
                new EffectData { EffectType = EffectType.PhysicalDamage, FloatAmount = new FloatReference(Damage) }
            ).Apply(source, target);
            BattleEvent.Publish(this);
        }
    }

    public void Apply(Member source, Member target) {
        this.Apply(source, new MemberAsTarget(target));
    }
}
