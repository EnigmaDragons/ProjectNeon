
public interface GlobalEffect
{
    string ShortDescriptionTerm { get; }
    string FullDescriptionTerm { get; }
    GlobalEffectData Data { get; }
    void Apply(GlobalEffectContext ctx);
    void Revert(GlobalEffectContext ctx);
}