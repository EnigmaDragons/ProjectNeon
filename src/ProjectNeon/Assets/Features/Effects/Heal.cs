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
        if (target.Members.Length > 1)
        {
            target.Members.ForEach(
                member => {
                    new Heal(_amount).Apply(source, member);
                }
            );
        }
        else
        {
            Debug.Log("Healing...");
            target.Members[0].State.GainHp(_amount);
        }
    }

    public void Apply(Member source, Member target)
    {
        Apply(source, new MemberAsTarget(target));
    }
}