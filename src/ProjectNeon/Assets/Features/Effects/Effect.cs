using System;
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
    public Maybe<Card> Card { get; }

    public EffectContext(Member source, Target target, Maybe<Card> card)
        : this(source, target, card, PartyAdventureState.InMemory(), new PlayerState(), target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)) {}
        
    public EffectContext(Member source, Target target, Maybe<Card> card, PartyAdventureState adventureState, PlayerState playerState, IDictionary<int, Member> battleMembers)
    {
        Source = source;
        Target = target;
        AdventureState = adventureState;
        PlayerState = playerState;
        BattleMembers = battleMembers;
        Card = card;
    }
}

public static class EffectExtensions
{
    public static void Apply(this Effect effect, Member source, Target target) 
        => effect.Apply(source, target, Maybe<Card>.Missing());
    
    public static void Apply(this Effect effect, Member source, Member target) 
        => effect.Apply(source, target, Maybe<Card>.Missing());

    public static void Apply(this Effect effect, Member source, Target target, Maybe<Card> card) 
        => effect.Apply(new EffectContext(source, target, card, PartyAdventureState.InMemory(), new PlayerState(0), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m)));
    
    public static void Apply(this Effect effect, Member source, Member target, Maybe<Card> card) 
        => effect.Apply(new EffectContext(source, new Single(target), card, PartyAdventureState.InMemory(), new PlayerState(0), 
            new [] { source, target }.SafeToDictionary(m => m.Id, m => m)));
}

public sealed class NoEffect : Effect
{
    public void Apply(EffectContext ctx) {}
}


public sealed class SimpleEffect : Effect
{
    private readonly Action<Member, Target> _apply;

    public SimpleEffect(Action apply) => _apply = (_, __) => apply();
    public SimpleEffect(Action<Member, MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, member))) { }
    public SimpleEffect(Action<MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(applyToOne)) {}
    public SimpleEffect(Action<Target> apply) : this((src, t) => apply(t)) {}
    public SimpleEffect(Action<Member, Target> apply) => _apply = apply;

    public void Apply(EffectContext ctx) => _apply(ctx.Source, ctx.Target);
}

public class FullContextEffect : Effect
{
    private readonly Action<EffectContext, Target> _apply;

    public FullContextEffect(Action apply) => _apply = (_, __) => apply();
    public FullContextEffect(Action<EffectContext, MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, member))) { }
    public FullContextEffect(Action<MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(applyToOne)) {}
    public FullContextEffect(Action<Target> apply) : this((src, t) => apply(t)) {}
    public FullContextEffect(Action<EffectContext, Target> apply) => _apply = apply;

    public void Apply(EffectContext ctx) => _apply(ctx, ctx.Target);
}
