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
    public ResourceQuantity PaidAmount { get; }
    public PlayerState PlayerState { get; }
    public PartyAdventureState AdventureState { get; }
    public BattleRewardState RewardState { get; }
    public IDictionary<int, Member> BattleMembers { get; } // This should probably be converted to be lazy so that it's accurate no matter when the Context is created
    public CardPlayZones PlayerCardZones { get; }
    public PreventionContext Preventions { get; }
    public SelectionContext Selections { get; }
    public IDictionary<int, CardTypeData> AllCards { get; }
    public int BattleStartingCredits { get; }
    public int CurrentCredits { get; }
    public IDictionary<int, EnemyType> EnemyTypes { get; }
    public Func<int> GetNextCardId { get; }
    public PlayedCardSnapshot[] CardsPlayedThisTurn { get; }
    public Dictionary<int, Color> OwnerTints { get; }
    public Dictionary<int, Sprite> OwnerBusts { get; }
    public bool IsReaction { get; }
    public ReactionTimingWindow Timing { get; }
    public EffectScopedData ScopedData { get; }

    public EffectContext(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, ResourceQuantity paidAmount,
        PartyAdventureState adventureState, PlayerState playerState, BattleRewardState rewardState, IDictionary<int, Member> battleMembers, 
        CardPlayZones playerCardZones, PreventionContext preventions, SelectionContext selections, IDictionary<int, CardTypeData> allCards, 
        int battleStartingCredits, int currentCredits, IDictionary<int, EnemyType> enemyTypes, Func<int> getNextCardId, PlayedCardSnapshot[] cardsPlayedThisTurn, 
        Dictionary<int, Color> ownerTints, Dictionary<int, Sprite> ownerBusts, bool isReaction, ReactionTimingWindow timing, 
        EffectScopedData scopedData)
    {
        Source = source;
        SourceSnapshot = source.GetSnapshot();
        SourceStateSnapshot = SourceSnapshot.State;
        Target = target;
        AdventureState = adventureState;
        PlayerState = playerState;
        RewardState = rewardState;
        BattleMembers = battleMembers;
        PlayerCardZones = playerCardZones;
        Card = card;
        XPaidAmount = xPaidAmount;
        PaidAmount = paidAmount;
        Preventions = preventions;
        Selections = selections;
        AllCards = allCards;
        BattleStartingCredits = battleStartingCredits;
        CurrentCredits = currentCredits;
        EnemyTypes = enemyTypes;
        GetNextCardId = getNextCardId;
        CardsPlayedThisTurn = cardsPlayedThisTurn;
        OwnerTints = ownerTints;
        OwnerBusts = ownerBusts;
        IsReaction = isReaction;
        Timing = timing;
        ScopedData = scopedData;
        if (XPaidAmount == null)
        {
            Log.Error("XPaidAmount is null");
            #if UNITY_EDITOR
            throw new ArgumentNullException();
            #endif
        }
    }
    
    public static EffectContext ForEffectFromBattleState(BattleState state, Member src, Target target, ReactionTimingWindow timing) 
        => new EffectContext(
            src,
            target,
            Maybe<Card>.Missing(), 
            ResourceQuantity.None, 
            ResourceQuantity.None, 
            state.Party, 
            state.PlayerState, 
            state.RewardState,
            state.Members,
            state.PlayerCardZones,
            new UnpreventableContext(), 
            new SelectionContext(), 
            new Dictionary<int, CardTypeData>(), // Weird. This might be needed.
            state.Party.Credits,
            state.Party.Credits, 
            new Dictionary<int, EnemyType>(), // Weird. This might be needed.
            state.GetNextCardId, 
            new PlayedCardSnapshot[0],
            state.OwnerTints,
            state.OwnerBusts,
            false,
            timing,
            state.EffectScopedData
        );

    public EffectContext Retargeted(Member source, Target target) 
        => new EffectContext(source, target, Card, XPaidAmount, PaidAmount, AdventureState, PlayerState, RewardState, BattleMembers, PlayerCardZones, 
            Preventions.WithUpdatedTarget(target), Selections, AllCards, BattleStartingCredits, CurrentCredits, EnemyTypes, GetNextCardId, CardsPlayedThisTurn, 
            OwnerTints, OwnerBusts, IsReaction, Timing, ScopedData);
    
    public EffectContext WithFreshPreventionContext() 
        => new EffectContext(Source, Target, Card, XPaidAmount, PaidAmount, AdventureState, PlayerState, RewardState, BattleMembers, PlayerCardZones, 
            new PreventionContextMut(Target), Selections, AllCards, BattleStartingCredits, CurrentCredits, EnemyTypes, GetNextCardId, CardsPlayedThisTurn, 
            OwnerTints, OwnerBusts, IsReaction, Timing, ScopedData);
    
    public EffectContext WithReactionTimingContext(ReactionTimingWindow timing)
        => new EffectContext(Source, Target, Card, XPaidAmount, PaidAmount, AdventureState, PlayerState, RewardState, BattleMembers, PlayerCardZones, 
            new PreventionContextMut(Target), Selections, AllCards, BattleStartingCredits, CurrentCredits, EnemyTypes, GetNextCardId, CardsPlayedThisTurn, 
            OwnerTints, OwnerBusts, IsReaction, timing, ScopedData);

    public static EffectContext ForTests(Member source, Target target)
        => ForTests(source, target, Maybe<Card>.Missing(), ResourceQuantity.None, ResourceQuantity.None, new UnpreventableContext());
    
    public static EffectContext ForTests(Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, ResourceQuantity paidAmount, PreventionContext preventions)
        => new EffectContext(source, target, card, xPaidAmount, paidAmount, PartyAdventureState.InMemory(), new PlayerState(), BattleRewardState.InMemory(),
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, preventions, new SelectionContext(), 
            new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0],
            new Dictionary<int, Color>(), new Dictionary<int, Sprite>(), false, ReactionTimingWindow.FirstCause, new EffectScopedData());

    public static EffectContext ForTests(Member source, Target target, CardPlayZones cardPlayZones, Dictionary<int, CardTypeData> allCards)
        => new EffectContext(source, target, Maybe<Card>.Missing(), ResourceQuantity.None, ResourceQuantity.None, PartyAdventureState.InMemory(),
            new PlayerState(0), BattleRewardState.InMemory(), new Dictionary<int, Member>(), cardPlayZones, new UnpreventableContext(), new SelectionContext(), 
            allCards, 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0], new Dictionary<int, Color>(),
            new Dictionary<int, Sprite>(), false, ReactionTimingWindow.FirstCause, new EffectScopedData());
    
    public static EffectContext ForTests(Member source, Target target, PartyAdventureState adventureState)
        => new EffectContext(source, target, Maybe<Card>.Missing(), ResourceQuantity.None, ResourceQuantity.None, adventureState,
            new PlayerState(0), BattleRewardState.InMemory(), new Dictionary<int, Member>(), CardPlayZones.InMemory, new UnpreventableContext(), new SelectionContext(), 
            new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, new PlayedCardSnapshot[0],
            new Dictionary<int, Color>(), new Dictionary<int, Sprite>(), false, ReactionTimingWindow.FirstCause, new EffectScopedData());
}

public static class EffectExtensions
{
    public static void ApplyForTests(this Effect effect, Member source, Member target) 
        => effect.ApplyForTests(source, new Single(target), Maybe<Card>.Missing(), ResourceQuantity.None, ResourceQuantity.None);

    private static void ApplyForTests(this Effect effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xAmountPaid, ResourceQuantity paidAmount) 
        => effect.Apply(new EffectContext(source, target, card, xAmountPaid, paidAmount, PartyAdventureState.InMemory(), new PlayerState(0), BattleRewardState.InMemory(),  
            target.Members.Concat(source).SafeToDictionary(m => m.Id, m => m), CardPlayZones.InMemory, new PreventionContextMut(target), 
            new SelectionContext(), new Dictionary<int, CardTypeData>(), 0, 0, new Dictionary<int, EnemyType>(), () => 0, 
            new PlayedCardSnapshot[0], new Dictionary<int, Color>(), new Dictionary<int, Sprite>(), false, ReactionTimingWindow.FirstCause, new EffectScopedData()));
}

public sealed class NoEffect : Effect
{
    public void Apply(EffectContext ctx) {}
}

public sealed class SimpleEffect : Effect
{
    private readonly Action<Member, Target> _apply;

    public SimpleEffect(Action apply) => _apply = (_, __) => apply();
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
        : this((src, t) => t.ApplyToAllConscious(member => applyToOne(src, Formula.EvaluateToInt(src.SourceSnapshot.State, member, durationFormula, src.XPaidAmount, src.ScopedData), member))) { }
    public FullContextEffect(Action<EffectContext, Target> apply) => _apply = apply;

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
            var formulaAmount = Formula.EvaluateToInt(ctx.SourceStateSnapshot, m.State, _formula, ctx.XPaidAmount, ctx.ScopedData);

            var isDebuff = _canAegisPreventForFormulaResult(formulaAmount);
            if (isDebuff) 
                ctx.Preventions.RecordPreventionTypeEffect(PreventionType.Aegis, m.AsArray());

            if (ctx.Preventions.IsAegising(m))
                BattleLog.Write($"{m.UnambiguousEnglishName} prevented the effect with an Aegis");
            else
                _applyToOne(ctx, formulaAmount, m.State);
        });
    }
}