using System;

public class BasicGlobalEffect : GlobalEffect
{
    private readonly GlobalEffectData _data;
    private readonly Action<CurrentGlobalEffects> _apply;
    private readonly Action<CurrentGlobalEffects> _revert;

    public BasicGlobalEffect(GlobalEffectData data, Action<CurrentGlobalEffects> apply, Action<CurrentGlobalEffects> revert)
    {
        _data = data;
        _apply = apply;
        _revert = revert;
    }

    public string ShortDescriptionTerm => _data.ShortDescriptionTerm;
    public string FullDescriptionTerm => _data.FullDescriptionTerm;
    public GlobalEffectData Data => _data;
    public void Apply(GlobalEffectContext ctx) => _apply(ctx.GlobalEffects);
    public void Revert(GlobalEffectContext ctx) => _revert(ctx.GlobalEffects);
}
