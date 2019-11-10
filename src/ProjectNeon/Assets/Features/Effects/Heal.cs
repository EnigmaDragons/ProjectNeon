using UnityEngine;

public sealed class Heal : Effect
{
    private int _amount;

    public Heal(int amount)
    {
        _amount = amount;
    }

    public void Apply(Member source, Target target)
    {
        target.Members.ForEach(
            member => {
                member.State.GainHp(_amount);
            }
        );
    }

    public void Apply(Member source, Member target)
    {
        Apply(source, new MemberAsTarget(target));
    }
}