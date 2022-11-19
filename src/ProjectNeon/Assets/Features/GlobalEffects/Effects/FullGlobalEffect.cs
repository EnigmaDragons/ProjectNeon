using System;

public class FullGlobalEffect : GlobalEffect
{
    private readonly GlobalEffectData _data;
    private readonly Action<GlobalEffectContext> _apply;
    private readonly Action<GlobalEffectContext> _revert;

    public FullGlobalEffect(GlobalEffectData data, Action<GlobalEffectContext> apply, Action<GlobalEffectContext> revert)
    {
        _data = data;
        _apply = apply;
        _revert = revert;
    }

    public string ShortDescriptionTerm => _data.ShortDescriptionTerm;
    public string FullDescriptionTerm => _data.FullDescriptionTerm;
    public GlobalEffectData Data => _data;
    public void Apply(GlobalEffectContext ctx) => _apply(ctx);
    public void Revert(GlobalEffectContext ctx) => _revert(ctx);
}
