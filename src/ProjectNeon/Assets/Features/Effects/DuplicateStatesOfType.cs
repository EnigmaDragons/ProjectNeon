
public class DuplicateStatesOfType : Effect
{
    private readonly StatusTag _tag;

    public DuplicateStatesOfType(StatusTag tag)
    {
        _tag = tag;
    }

    public void Apply(EffectContext ctx)
    {
        if (_tag == StatusTag.None)
            return;

        ctx.Target.ApplyToAllConscious(m => m.DuplicateStatesOfType(_tag));
    }
}
