using System.Collections.Generic;

public interface Effect
{
    void Apply(EffectContext ctx);
}

public class EffectContext
{
    public Member Source { get; }
    public Target Target { get; }
    public PlayerState PlayerState { get; }
    public PartyAdventureState AdventureState { get; }
    public IDictionary<int, Member> BattleMembers { get; }

    public EffectContext(Member source, Target target)
        : this(source, target, PartyAdventureState.InMemory(), new PlayerState(), target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)) {}
        
    public EffectContext(Member source, Target target, PartyAdventureState adventureState, PlayerState playerState, IDictionary<int, Member> battleMembers)
    {
        Source = source;
        Target = target;
        AdventureState = adventureState;
        PlayerState = playerState;
        BattleMembers = battleMembers;
    }
}

public static class EffectExtensions
{
    public static void Apply(this Effect effect, Member source, Target target) 
        => effect.Apply(new EffectContext(source, target, PartyAdventureState.InMemory(), new PlayerState(), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)));
    
    public static void Apply(this Effect effect, Member source, Member target) 
        => effect.Apply(new EffectContext(source, new Single(target), PartyAdventureState.InMemory(), new PlayerState(), 
            new [] { source, target }.SafeToDictionary(m => m.Id, m => m)));
}
