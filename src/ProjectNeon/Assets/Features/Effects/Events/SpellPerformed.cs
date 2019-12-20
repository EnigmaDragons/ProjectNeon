
public sealed class SpellPerformed
{
    public SpellFlatDamageEffect SpellFlatDamageEffect { get; }

    public SpellPerformed(SpellFlatDamageEffect spell)
    {
        SpellFlatDamageEffect = spell;
    }
}

