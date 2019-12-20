
public sealed class SpellToPerform
{
    public SpellFlatDamageEffect SpellFlatDamageEffect { get; }

    public SpellToPerform(SpellFlatDamageEffect spell)
    {
        SpellFlatDamageEffect = spell;
    }
}

