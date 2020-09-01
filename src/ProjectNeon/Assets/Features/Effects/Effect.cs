using System.Collections.Generic;
using System.Linq;

public interface Effect
{
    void Apply(EffectContext ctx);
}

public class EffectContext
{
    public Member Source { get; }
    public Target Target { get; }
    public PlayerState PlayerState { get; }
    public IDictionary<int, Member> BattleMembers { get; }

    public EffectContext(Member source, Target target)
        : this(source, target, new PlayerState(), target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)) {}
        
    public EffectContext(Member source, Target target, PlayerState playerState, IDictionary<int, Member> battleMembers)
    {
        Source = source;
        Target = target;
        PlayerState = playerState;
        BattleMembers = battleMembers;
    }
}

public static class EffectExtensions
{
    public static void Apply(this Effect effect, Member source, Target target) 
        => effect.Apply(new EffectContext(source, target, new PlayerState(), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)));
    
    public static void Apply(this Effect effect, Member source, Member target) 
        => effect.Apply(new EffectContext(source, new Single(target), new PlayerState(), 
            new [] { source, target }.SafeToDictionary(m => m.Id, m => m)));
}
