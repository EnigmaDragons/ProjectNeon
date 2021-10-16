using System;

public class FullGlobalEffect : GlobalEffect
{
    private readonly GlobalEffectData _data;
    private readonly Action<GlobalEffectContext> _apply;
    
    public FullGlobalEffect(GlobalEffectData data, Action<GlobalEffectContext> apply)
    {
        _data = data;
        _apply = apply;
    }

    public string ShortDescription => _data.ShortDescription;
    public string FullDescription => _data.FullDescription;
    public void Apply(GlobalEffectContext ctx) => _apply(ctx);
}
