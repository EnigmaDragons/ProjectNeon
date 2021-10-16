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

    public string ShortDescription => _data.ShortDescription;
    public string FullDescription => _data.FullDescription;
    public GlobalEffectData Data => _data;
    public void Apply(GlobalEffectContext ctx) => _apply(ctx);
    public void Revert(GlobalEffectContext ctx) => _revert(ctx);
}
