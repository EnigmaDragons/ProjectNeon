public class ProposedEffect
{
    public EffectData EffectData { get; }
    public Member Source { get; }
    public Target Target { get; }

    public void Execute() => AllEffects.Apply(EffectData, Source, Target);
}
