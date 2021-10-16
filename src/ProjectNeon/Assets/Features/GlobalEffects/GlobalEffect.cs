
public interface GlobalEffect
{
    string ShortDescription { get; }
    string FullDescription { get; }
    void Apply(GlobalEffectContext ctx);
    void Revert(GlobalEffectContext ctx);
}