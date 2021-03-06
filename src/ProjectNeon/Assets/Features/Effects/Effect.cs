﻿using System;
using System.Collections.Generic;
using UnityEngine;

public interface Effect
{
    void Apply(EffectContext ctx);
}

public class EffectContext
{
    public Member Source { get; }
    public MemberSnapshot SourceSnapshot { get; }
    public MemberStateSnapshot SourceStateSnapshot { get; }
    public Target Target { get; }
    public Maybe<Card> Card { get; }
    public ResourceQuantity XPaidAmount { get; }
    public PlayerState PlayerState { get; }
    public PartyAdventureState AdventureState { get; }
    public IDictionary<int, Member> BattleMembers { get; } // This should probably be converted to be lazy so that it's accurate no matter when the Context is created
    public CardPlayZones PlayerCardZones { get; }
    public PreventionContext Preventions { get; }
    public SelectionContext Selections { get; }
    public IDictionary<int, CardTypeData> AllCards { get; }
    public int StartingCredits { get; }
    public int CurrentCredits { get; }
    public IDictionary<int, EnemyType> EnemyTypes { get; }
    public Func<int> GetNextCardId { get; }
    public PlayedCardSnapshot[] CardsPlayedThisTurn { get; }

    public EffectContext(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PartyAdventureState adventureState, 
        PlayerState playerState, IDictionary<int, Member> battleMembers, CardPlayZones playerCardZones, PreventionContext preventions, 
        SelectionContext selections, IDictionary<int, CardTypeData> allCards, int startingCredits, int currentCredits, IDictionary<int, EnemyType> enemyTypes,
        Func<int> getNextCardId, PlayedCardSnapshot[] cardsPlayedThisTurn)
    {
        Source = source;
        SourceSnapshot = source.GetSnapshot();
        SourceStateSnapshot = SourceSnapshot.State;
        Target = target;
        AdventureState = adventureState;
        PlayerState = playerState;
        BattleMembers = battleMembers;
        PlayerCardZones = playerCardZones;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
        Selections = selections;
        AllCards = allCards;
        StartingCredits = startingCredits;
        CurrentCredits = currentCredits;
        EnemyTypes = enemyTypes;
        GetNextCardId = getNextCardId;
        CardsPlayedThisTurn = cardsPlayedThisTurn;
        if (XPaidAmount == null)
        {
            Log.Error("XPaidAmount is null");
            #if UNITY_EDITOR
            throw new ArgumentNullException();
            #endif
        }
    }
    
    public EffectContext Retargeted(Member source, Target target) 
        => new EffectContext(source, target, Card, XPaidAmount, AdventureState, PlayerState, BattleMembers, PlayerCardZones, 
            Preventions, Selections, AllCards, StartingCredits, CurrentCredits, EnemyTypes, GetNextCardId, CardsPlayedThisTurn);

    public static EffectContext ForTests(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PreventionContext preventions)
        => new EffectContext(source, target, card, xPaidAmount, PartyAdventureState.InMemory(), new PlayerState(), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, preventions, new SelectionContext(), 
            new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0]);

    public static EffectContext ForTests(Member source, Target target, CardPlayZones cardPlayZones, Dictionary<int, CardTypeData> allCards)
        => new EffectContext(source, target, Maybe<Card>.Missing(), ResourceQuantity.None, PartyAdventureState.InMemory(),
            new PlayerState(0), new Dictionary<int, Member>(), cardPlayZones, new UnpreventableContext(), new SelectionContext(), 
            allCards, 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0]);
    
    public static EffectContext ForTests(Member source, Target target, PartyAdventureState adventureState)
        => new EffectContext(source, target, Maybe<Card>.Missing(), ResourceQuantity.None, adventureState,
            new PlayerState(0), new Dictionary<int, Member>(), CardPlayZones.InMemory, new UnpreventableContext(), new SelectionContext(), 
            new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0]);
}

public static class EffectExtensions
{
    public static void ApplyForTests(this Effect effect, Member source, Member target) 
        => effect.ApplyForTests(source, new Single(target), Maybe<Card>.Missing(), ResourceQuantity.None);

    private static void ApplyForTests(this Effect effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xAmountPaid) 
        => effect.Apply(new EffectContext(source, target, card, xAmountPaid, PartyAdventureState.InMemory(), new PlayerState(0), 
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, new PreventionContextMut(target), 
            new SelectionContext(), new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0]));
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

    public FullContextEffect(Action<EffectContext, int> apply, string durationFormula) 
        : this((ctx, duration, m) => apply(ctx, duration), durationFormula) {}
    public FullContextEffect(Action<EffectContext, int, MemberState> applyToOne, string durationFormula) 
        : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, Mathf.CeilToInt(Formula.Evaluate(src.SourceSnapshot.State, member, durationFormula, src.XPaidAmount)), member))) { }
    private FullContextEffect(Action<EffectContext, Target> apply) => _apply = apply;

    public void Apply(EffectContext ctx) => _apply(ctx, ctx.Target);
}

public class AegisIfFormulaResult : Effect
{
    private readonly Action<EffectContext, float, MemberState> _applyToOne;
    private readonly string _formula;
    private readonly bool _shouldRoundUp;
    private readonly Func<float, bool> _canAegisPreventForFormulaResult;

    public AegisIfFormulaResult(Action<EffectContext, float, MemberState> applyToOne, string formula, bool shouldRoundUp,
        Func<float, bool> canAegisPreventForFormulaResult)
    {
        _applyToOne = applyToOne;
        _formula = formula;
        _shouldRoundUp = shouldRoundUp;
        _canAegisPreventForFormulaResult = canAegisPreventForFormulaResult;
    }

    public void Apply(EffectContext ctx)
    {
        ctx.Target.Members.GetConscious().ForEach(m =>
        {
            var formulaAmount = Formula.Evaluate(ctx.SourceStateSnapshot, m.State, _formula, ctx.XPaidAmount);
            if (_shouldRoundUp)
                formulaAmount = formulaAmount.CeilingInt();

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