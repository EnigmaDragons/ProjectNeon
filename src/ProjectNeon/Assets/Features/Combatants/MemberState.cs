using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private int _versionNumber;

    private readonly Dictionary<string, BattleCounter> _counters =
        new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);

    private readonly DictionaryWithDefault<CardTag, int> _tagsPlayedCount 
        = new DictionaryWithDefault<CardTag, int>(0);

    private readonly IStats _baseStats;
    private readonly List<IPersistentState> _persistentStates = new List<IPersistentState>();
    private readonly List<ITemporalState> _additiveMods = new List<ITemporalState>();
    private readonly List<ITemporalState> _multiplierMods = new List<ITemporalState>();
    private readonly List<ReactiveStateV2> _reactiveStates = new List<ReactiveStateV2>();
    private readonly List<EffectTransformer> _transformers = new List<EffectTransformer>();
    private readonly List<IBonusCardPlayer> _bonusCardPlayers = new List<IBonusCardPlayer>();
    private readonly List<ResourceCalculator> _additiveResourceCalculators = new List<ResourceCalculator>();
    private readonly List<ResourceCalculator> _multiplicativeResourceCalculators = new List<ResourceCalculator>();
    private readonly List<CustomStatusIcon> _customStatusIcons = new List<CustomStatusIcon>();

    public IStats BaseStats => _baseStats;
    public StatType PrimaryStat { get; }

    private IStats CurrentStats => _baseStats
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats))
        .Times(_multiplierMods.Where(x => x.IsActive).Select(x => x.Stats))
        .NotBelow(0)
        .WithWholeNumbersWhereExpected();

    private BattleCounter Counter(string name) => _counters.VerboseGetValue(name, n => $"Counter '{n}'");
    private BattleCounter Counter(StatType statType) => _counters[statType.ToString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.ToString()];

    public int MemberId { get; }
    public string Name { get; }

    public MemberState(int id, string name, IStats baseStats, StatType primaryStat)
        : this(id, name, baseStats, primaryStat, baseStats.MaxHp())
    {
    }

    public MemberState(int id, string name, IStats baseStats, StatType primaryStat, int initialHp)
    {
        MemberId = id;
        Name = name;
        PrimaryStat = primaryStat;
        _baseStats = baseStats;

        _counters[TemporalStatType.HP.ToString()] =
            new BattleCounter(TemporalStatType.HP, initialHp, () => CurrentStats.MaxHp());
        _counters[TemporalStatType.Shield.ToString()] =
            new BattleCounter(TemporalStatType.Shield, baseStats[StatType.StartingShield], () => CurrentStats.MaxShield());
        Enum.GetValues(typeof(TemporalStatType))
            .Cast<TemporalStatType>()
            .Skip(2)
            .ForEach(t => _counters[t.ToString()] = new BattleCounter(t, 0, () => 999));
        _counters[TemporalStatType.Phase.ToString()].Set(1);

        baseStats.ResourceTypes?.ForEach(r =>
            _counters[r.Name] = new BattleCounter(r.Name, Math.Max(0, r.StartingAmount), () => r.MaxAmount));
        if (baseStats.ResourceTypes.Length > 0)
            _counters["PrimaryResource"] = _counters[baseStats.ResourceTypes[0].Name];
        
        _counters["None"] = new BattleCounter("None", 0, () => 0);
        _counters[""] = new BattleCounter("", 0, () => 0);
    }

    public void InitResourceAmount(IResourceType resourceType, int amount) => _counters[resourceType.Name].Set(amount);

    // Queries
    public MemberStateSnapshot ToSnapshot()
        => new MemberStateSnapshot(_versionNumber, MemberId, CurrentStats.ToSnapshot(),
            _counters.ToDictionary(c => c.Key, c => c.Value.Amount), ResourceTypes, _tagsPlayedCount,
                new DictionaryWithDefault<StatusTag, int>(0, Enum.GetValues(typeof(StatusTag)).Cast<StatusTag>()
                    .SafeToDictionary(s => s, s => StatusesOfType(s).Length)), PrimaryStat);
    
    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => _counters.TryGetValue(resourceType.Name, out var r) ? r.Amount : 0;
    public int ResourceAmount(string resourceType) => _counters[resourceType].Amount;
    public float this[StatType statType] => statType switch
    {
        StatType.Damagability => CurrentStats.Damagability() + (IsVulnerable() ? 0.33f : 0),
        StatType.Healability => CurrentStats.Healability() + (IsAntiHeal() ? -0.5f : 0),
        _ => CurrentStats[statType]
    };
    private bool IsVulnerable() => Counter(TemporalStatType.Vulnerable).Amount > 0;
    private bool IsAntiHeal() => Counter(TemporalStatType.AntiHeal).Amount > 0;
    public float this[TemporalStatType statType] => _counters[statType.ToString()].Amount + CurrentStats[statType];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
    public float Max(string name) => _counters.TryGetValue(name, out var c) ? c.Max : 0;
    public IResourceType PrimaryResource => ResourceTypes.Any() ? ResourceTypes[0] : new InMemoryResourceType();
    public int PrimaryResourceAmount => ResourceTypes.Any() ? _counters[PrimaryResource.Name].Amount : 0;

    public ResourceQuantity CurrentPrimaryResources => new ResourceQuantity
        {Amount = PrimaryResourceAmount, ResourceType = PrimaryResource.Name};

    public float PrimaryResourceValue => 1f / 4f;

    public int DifferenceFromBase(StatType statType) => (CurrentStats[statType] - _baseStats[statType]).CeilingInt();

    public bool HasStatus(StatusTag tag) => _reactiveStates.Any(r => r.Status.Tag == tag)
                                            || _additiveMods.Any(r => r.Status.Tag == tag)
                                            || _multiplierMods.Any(r => r.Status.Tag == tag);

    public ITemporalState[] StatusesOfType(StatusTag tag)
        => TemporalStates.Where(s => s.Status.Tag == tag).ToArray();

    public CustomStatusIcon[] CustomStatuses()
        => _customStatusIcons.ToArray();
    
    // Bonus Cards 
    public CardType[] GetBonusCards(BattleStateSnapshot snapshot)
        => _bonusCardPlayers
            .Select(x => x.GetBonusCardOnResolutionPhaseBegun(snapshot))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();

    // Reaction Commands
    public ProposedReaction[] GetReactions(EffectResolved e) =>
        ApplicableReactiveStates
            .Select(x => x.OnEffectResolved(e))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();

    private IEnumerable<ReactiveStateV2> ApplicableReactiveStates =>
        this[TemporalStatType.Disabled] > 0 || this[TemporalStatType.Stun] > 0
            ? _reactiveStates.Where(x => x.Status.Tag != StatusTag.CounterAttack)
            : _reactiveStates;
    
    public EffectData Transform(EffectData effect, EffectContext context)
    {
        foreach (var transformer in _transformers)
            effect = transformer.Modify(effect, context);
        return effect;
    }

    public ResourceCalculations CalculateResources(CardTypeData card)
    {
        var calc = card.CalculateResources(this);
        var additives = _additiveResourceCalculators.Select(x => x.GetModifiers(card, this)).ToArray();
        var multiplicatives = _multiplicativeResourceCalculators.Select(x => x.GetModifiers(card, this)).ToArray();
        return new ResourceCalculations(
            calc.ResourcePaidType, (calc.ResourcesPaid + additives.Sum(x => x.ResourcesPaid)) * multiplicatives.Product(x => x.ResourcesPaid),
            (calc.XAmount + additives.Sum(x => x.XAmount)) * multiplicatives.Product(x => x.XAmount),
            (calc.XAmountPriceTag + additives.Sum(x => x.XAmountPriceTag)) *
            multiplicatives.Product(x => x.XAmountPriceTag));
    }

    public void RecordUsage(Card card)
    {
        _additiveResourceCalculators.ForEach(x => x.RecordUsageIfApplicable(card));
        _multiplicativeResourceCalculators.ForEach(x => x.RecordUsageIfApplicable(card));
        foreach (var tag in card.Type.Tags)
            _tagsPlayedCount[tag]++;
    }

    public void DuplicateStatesOfType(StatusTag tag) 
        => DuplicateStatesOfTypeFrom(tag, this);

    public void DuplicateStatesOfTypeFrom(StatusTag tag, MemberState member)
    {
        member._additiveMods.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(ApplyTemporaryAdditive);
        member._multiplierMods.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(ApplyTemporaryMultiplier);
        member._reactiveStates.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddReactiveState((ReactiveStateV2) s));
        member._transformers.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddEffectTransformer((EffectTransformer) s));
        member._additiveResourceCalculators.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddAdditiveResourceCalculator((ResourceCalculator) s));
        member._multiplicativeResourceCalculators.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddMultiplicativeResourceCalculator((ResourceCalculator) s));
    }

    public void ResetStatToBase(string statType) => PublishAfter(() =>
    {
        _counters[statType].Set(0);
        if (Enum.TryParse(statType, out TemporalStatType tst))
        {
            _additiveMods.RemoveAll(m => m.Stats[tst] > 0);
            _multiplierMods.RemoveAll(m => m.Stats[tst] > 1);
        }
        if (Enum.TryParse(statType, out StatType t))
        {
            _additiveMods.RemoveAll(m => m.Stats[t] > 0);
            _multiplierMods.RemoveAll(m => m.Stats[t] > 1);
        }
    });
    
    public void ApplyBonusCardPlayer(IBonusCardPlayer p) => _bonusCardPlayers.Add(p);
    public void ApplyPersistentState(IPersistentState state) => _persistentStates.Add(state);

    public void ApplyTemporaryAdditive(ITemporalState mods) => PublishAfter(() =>
    {
        _additiveMods.Add(mods);
        if (CurrentStats.MaxHp() < CurrentStats.Hp())
            SetHp(CurrentStats.MaxHp());
    });

    public IEnumerable<ITemporalState> DamageOverTimes() => StatusesOfType(StatusTag.DamageOverTime);

    public void ApplyTemporaryMultiplier(ITemporalState mods) => PublishAfter(() =>
    {
        _multiplierMods.Add(mods);
    });

    private static readonly TemporalStatType[] DebuffStatTypes = { 
        TemporalStatType.Disabled, 
        TemporalStatType.Confused, 
        TemporalStatType.Blind, 
        TemporalStatType.Inhibit,
        TemporalStatType.Stun
    };

    private IEnumerable<ITemporalState> TemporalStates => _additiveMods
        .Concat(_multiplierMods)
        .Concat(_reactiveStates)
        .Concat(_transformers)
        .Concat(_bonusCardPlayers)
        .Concat(_additiveResourceCalculators)
        .Concat(_multiplicativeResourceCalculators);
    
    private int GetNumDebuffs()
    {
        return DebuffStatTypes.Sum(t => _counters[t.ToString()].Amount)
            + _preventedTags.Count 
            + TemporalStates.Count(x => x.IsDebuff);
    }
    
    public int CleanseDebuffs() => -Diff(PublishAfter(() =>
    {
        DebuffStatTypes.ForEach(d => _counters[d.ToString()].Set(0));
        _preventedTags = new Dictionary<CardTag, int>();
        RemoveTemporaryEffects(s => s.IsDebuff);
    }, GetNumDebuffs));

    public void BreakStealth() => PublishAfter(() =>
    {
        RemoveTemporaryEffects(t => t.Status.Tag == StatusTag.Stealth);
        _counters[TemporalStatType.Stealth.ToString()].Set(0);
        _counters[TemporalStatType.Prominent.ToString()].Set(1);
    });
    
    public int RemoveTemporaryEffects(Predicate<ITemporalState> condition) => PublishAfter(() => 
        _additiveMods.RemoveAll(condition)
        + _multiplierMods.RemoveAll(condition)
        + _reactiveStates.RemoveAll(condition)
        + _transformers.RemoveAll(condition)
        + _additiveResourceCalculators.RemoveAll(condition)
        + _multiplicativeResourceCalculators.RemoveAll(condition));
    
    public void AddReactiveState(ReactiveStateV2 state) => PublishAfter(() => _reactiveStates.Add(state));
    public void RemoveReactiveState(ReactiveStateV2 state) => PublishAfter(() => _reactiveStates.Remove(state));
    public void AddEffectTransformer(EffectTransformer transformer) => PublishAfter(() => _transformers.Add(transformer));
    public void AddAdditiveResourceCalculator(ResourceCalculator calculator) => PublishAfter(() => _additiveResourceCalculators.Add(calculator));
    public void AddMultiplicativeResourceCalculator(ResourceCalculator calculator) => PublishAfter(() => _multiplicativeResourceCalculators.Add(calculator));
    public void AddCustomStatus(CustomStatusIcon icon) => PublishAfter(() => _customStatusIcons.Add(icon));

    // HP Commands
    public void GainHp(float amount) => ChangeHp(amount * this[StatType.Healability]);
    public void SetHp(float amount) => PublishAfter(() => Counter(TemporalStatType.HP).Set(amount));
    public int TakeRawDamage(int amount) => -ChangeHp(-amount * this[StatType.Damagability]);
    public void TakeDamage(int amount)
    {
        var clampedAmount = Math.Max(amount, 0);
        var shieldModificationAmount = Math.Min(clampedAmount, Counter(TemporalStatType.Shield).Amount);
        clampedAmount -= shieldModificationAmount;
        if (shieldModificationAmount > 0)
            AdjustShield(-shieldModificationAmount);
        if (clampedAmount > 0)
            ChangeHp(-clampedAmount);
    }
    private int ChangeHp(float amount) => Diff(PublishAfter(() =>
    {
        if (this[TemporalStatType.PreventDeath] > 0)
            amount = Math.Max(amount, 1 - this[TemporalStatType.HP]);
        if (amount < 0)
            Message.Publish(new CharacterAnimationRequested2(MemberId, CharacterAnimationType.WhenHit) { Condition = Maybe<EffectCondition>.Missing() });
        Counter(TemporalStatType.HP).ChangeBy(amount);
    }, () => Counter(TemporalStatType.HP).Amount));

    // Status Counters Commands
    public void Adjust(string counterName, float amount) 
        => BattleLog.WriteIf(
            Diff(PublishAfter(() => Counter(counterName).ChangeBy(amount), () => Counter(counterName).Amount)), 
            v => $"{Name}'s {counterName} adjusted by {v}", 
            v => v != 0);
    public int Adjust(TemporalStatType t, float amount) => Diff(PublishAfter(() => Counter(t.ToString()).ChangeBy(amount), () => this[t].CeilingInt()));
    public int AdjustShield(float amount) => Adjust(TemporalStatType.Shield, amount);
    private void AdjustShieldNoPublish(float amount) => Counter(TemporalStatType.Shield.ToString()).ChangeBy(amount);

    private int Diff(int[] beforeAndAfter) => beforeAndAfter.Last() - beforeAndAfter.First();

    //Prevented Tag Commands
    private Dictionary<CardTag, int> _preventedTags = new Dictionary<CardTag, int>();
    public void PreventCardTag(CardTag tag, int amount)
    {
        if (!_preventedTags.ContainsKey(tag))
            _preventedTags[tag] = 0;
        _preventedTags[tag] += amount;
    } 
    public bool IsPrevented(HashSet<CardTag> tags) => tags.Any(x => _preventedTags.ContainsKey(x) && _preventedTags[x] > 0);
    public void ReducePreventedTagCounters() => _preventedTags.ToList().ForEach(x => _preventedTags[x.Key] = x.Value > 0 ? x.Value - 1 : 0);
    
    // Resource Commands
    public void Gain(ResourceQuantity qty, PartyAdventureState partyState) => GainResource(qty.ResourceType, qty.Amount, partyState);
    public void GainResource(string resourceName, int amount, PartyAdventureState partyState) => PublishAfter(() =>
    {
        if (amount == 0)
            return;
        if (resourceName == "Creds")
            partyState.UpdateCreditsBy(amount);
        else if (this[TemporalStatType.PreventResourceGains] == 0)
            Counter(resourceName).ChangeBy(amount);
    });
    
    public void AdjustPrimaryResource(int numToGive)
    {
        if (this[TemporalStatType.PreventResourceGains] == 0)
        {
            BattleLog.Write($"{Name} {BattleLog.GainedOrLostTerm(numToGive)} {numToGive} {PrimaryResource.Name}");
            PublishAfter(() => _counters[PrimaryResource.Name].ChangeBy(numToGive));
        }
    }

    public void Lose(ResourceQuantity qty, PartyAdventureState partyState) => LoseResource(qty.ResourceType, qty.Amount, partyState);
    // TODO: Combine Lose and Gain into one method
    private void LoseResource(string resourceName, int amount, PartyAdventureState partyState) => PublishAfter(() =>
    {        
        if (amount == 0)
            return;
        if (resourceName == "Creds")
            partyState.UpdateCreditsBy(-amount);
        else
            Counter(resourceName).ChangeBy(-amount);
    });

    public bool HasAnyTemporalStates => _additiveMods.Any() || _multiplierMods.Any() || _reactiveStates.Any() || _persistentStates.Any() || _temporalStatsToReduceAtEndOfTurn.Any(x => this[x] > 0); 
    public IPayloadProvider[] GetTurnStartEffects()
    {
        _persistentStates.ForEach(m => m.OnTurnStart());
        return _additiveMods.Select(m => m.OnTurnStart())
            .Concat(_multiplierMods.Select(m => m.OnTurnStart()))
            .Concat(_reactiveStates.Select(m => m.OnTurnStart()))
            .Concat(_transformers.Select(m => m.OnTurnStart()))
            .Concat(_additiveResourceCalculators.Select(m => m.OnTurnStart()))
            .Concat(_multiplicativeResourceCalculators.Select(m => m.OnTurnStart()))
            .ToArray();
    }

    public void CleanExpiredStates() => 
        PublishAfter(() =>
        {
            var count = _additiveMods.RemoveAll(m => !m.IsActive)
                + _multiplierMods.RemoveAll(m => !m.IsActive)
                + _reactiveStates.RemoveAll(m => !m.IsActive)
                + _transformers.RemoveAll(m => !m.IsActive)
                + _additiveResourceCalculators.RemoveAll(m => !m.IsActive)
                + _multiplicativeResourceCalculators.RemoveAll(m => !m.IsActive);
            if (count > 0)
                DevLog.Write($"Cleaned {count} expired states from {Name}");
        });

    private readonly List<TemporalStatType> _temporalStatsToReduceAtEndOfTurn = new List<TemporalStatType>
    {
        TemporalStatType.Taunt,
        TemporalStatType.Stealth, // Should this be reduced?
        TemporalStatType.Confused,
        TemporalStatType.Prominent,
        TemporalStatType.PreventResourceGains,
        TemporalStatType.Vulnerable,
        TemporalStatType.AntiHeal
    };

    private void WithOtherAdjustmentRulesApplied(MemberStateSnapshot before)
    {
        if (this[TemporalStatType.Stealth] > before[TemporalStatType.Stealth])
            Counter(TemporalStatType.Taunt).Set(0);
        if (before[TemporalStatType.Stealth] > 0 && this[TemporalStatType.Taunt] > before[TemporalStatType.Taunt])
            BreakStealth();
    }
    
    public IPayloadProvider[] GetTurnEndEffects()
    {
        PublishAfter(() =>
        {
            _persistentStates.ForEach(m => m.OnTurnEnd());
            _temporalStatsToReduceAtEndOfTurn.ForEach(s => _counters[s.ToString()].ChangeBy(-1));
            _customStatusIcons.ForEach(m => m.StateTracker.AdvanceTurn());
            _customStatusIcons.RemoveAll(m => !m.StateTracker.IsActive);
            ReducePreventedTagCounters();
        });
        
        return _additiveMods.Select(m => m.OnTurnEnd())
            .Concat(_multiplierMods.Select(m => m.OnTurnEnd()))
            .Concat(_reactiveStates.Select(m => m.OnTurnEnd()))
            .Concat(_transformers.Select(m => m.OnTurnEnd()))
            .Concat(_additiveResourceCalculators.Select(m => m.OnTurnEnd()))
            .Concat(_multiplicativeResourceCalculators.Select(m => m.OnTurnEnd()))
            .ToArray();;
    }
    
    private void PublishAfter(Action action)
    {
        var before = ToSnapshot();
        _versionNumber++;
        action();
        WithOtherAdjustmentRulesApplied(before);
        Message.Publish(new MemberStateChanged(before, this));
    }
    
    private T PublishAfter<T>(Func<T> getResult)
    {
        var before = ToSnapshot();
        _versionNumber++;
        var result = getResult();
        WithOtherAdjustmentRulesApplied(before);
        Message.Publish(new MemberStateChanged(before, this));
        return result;
    }
    
    private T[] PublishAfter<T>(Action action, Func<T> getVal)
    {
        var before = ToSnapshot();
        var beforeVal = getVal();
        _versionNumber++;
        action();
        WithOtherAdjustmentRulesApplied(before);
        var afterVal = getVal();
        Message.Publish(new MemberStateChanged(before, this));
        return new T[] {beforeVal, afterVal};
    }
}
