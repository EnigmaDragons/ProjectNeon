﻿using System;
using System.Collections.Generic;

public interface Effect
{
    void Apply(EffectContext ctx);
}

public class EffectContext
{
    public Member Source { get; }
    public MemberSnapshot SourceSnapshot { get; }
    public Target Target { get; }
    public Maybe<Card> Card { get; }
    public ResourceQuantity XPaidAmount { get; }
    public PlayerState PlayerState { get; }
    public PartyAdventureState AdventureState { get; }
    public IDictionary<int, Member> BattleMembers { get; } // This should probably be converted to be lazy so that it's accurate no matter when the Context is created
    public CardPlayZones PlayerCardZones { get; }
    public PreventionContext Preventions { get; }

    public EffectContext(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PartyAdventureState adventureState, 
        PlayerState playerState, IDictionary<int, Member> battleMembers, CardPlayZones playerCardZones, PreventionContext preventions)
    {
        Source = source;
        SourceSnapshot = source.GetSnapshot();
        Target = target;
        AdventureState = adventureState;
        PlayerState = playerState;
        BattleMembers = battleMembers;
        PlayerCardZones = playerCardZones;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
        if (XPaidAmount == null)
        {
            Log.Error("XPaidAmount is null");
            #if UNITY_EDITOR
            throw new ArgumentNullException();
            #endif
        }
    }
    
    public EffectContext Retargeted(Member source, Target target) 
        => new EffectContext(source, target, Card, XPaidAmount, AdventureState, PlayerState, BattleMembers, PlayerCardZones, Preventions);

    public static EffectContext ForTests(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PreventionContext preventions)
        => new EffectContext(source, target, card, xPaidAmount, PartyAdventureState.InMemory(), new PlayerState(), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, preventions);
}

public static class EffectExtensions
{
    public static void ApplyForTests(this Effect effect, Member source, Member target) 
        => effect.ApplyForTests(source, new Single(target), Maybe<Card>.Missing(), ResourceQuantity.None);

    private static void ApplyForTests(this Effect effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xAmountPaid) 
        => effect.Apply(new EffectContext(source, target, card, xAmountPaid, PartyAdventureState.InMemory(), new PlayerState(0), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, new PreventionContextMut(target)));
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

    public FullContextEffect(Action<EffectContext> apply) => _apply = (ctx, __) => apply(ctx);
    public FullContextEffect(Action<EffectContext, MemberState> applyToOne) : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, member))) { }
    private FullContextEffect(Action<EffectContext, Target> apply) => _apply = apply;

    public void Apply(EffectContext ctx) => _apply(ctx, ctx.Target);
}

public class AegisIfFormulaResult : Effect
{
    private readonly Action<EffectContext, float, MemberState> _applyToOne;
    private readonly string _formula;
    private readonly Func<float, bool> _canAegisPreventForFormulaResult;

    public AegisIfFormulaResult(Action<EffectContext, float, MemberState> applyToOne, string formula, 
        Func<float, bool> canAegisPreventForFormulaResult)
    {
        _applyToOne = applyToOne;
        _formula = formula;
        _canAegisPreventForFormulaResult = canAegisPreventForFormulaResult;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var formulaAmount = Formula.Evaluate(ctx.Source, m, _formula, ctx.XPaidAmount);

            var isDebuff = _canAegisPreventForFormulaResult(formulaAmount);
            if (isDebuff) 
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.Name} prevented effect with an Aegis");
            else
                _applyToOne(ctx, formulaAmount, m.State);
        });
    }
}