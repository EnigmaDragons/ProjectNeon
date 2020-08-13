
public interface Effect
{
    void Apply(Member source, Target target);
}

public static class EffectExtensions
{
    public static void Apply(this Effect effect, Member source, Member target) 
        => effect.Apply(source, new Single(target));
}
