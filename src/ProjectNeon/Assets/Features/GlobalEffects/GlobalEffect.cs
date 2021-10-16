
public interface GlobalEffect
{
    string ShortDescription { get; }
    string FullDescription { get; }
    GlobalEffectData Data { get; }
    void Apply(GlobalEffectContext ctx);
    void Revert(GlobalEffectContext ctx);
}