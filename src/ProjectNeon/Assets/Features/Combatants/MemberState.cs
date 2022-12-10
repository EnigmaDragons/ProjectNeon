using System;
using System.Collections.Generic;
using System.Linq;

public sealed class MemberState : IStats
{
    private int _versionNumber;

    private readonly Dictionary<string, BattleCounter> _counters = new Dictionary<string, BattleCounter>();

    private readonly DictionaryWithDefault<CardTag, int> _tagsPlayedCount = new DictionaryWithDefault<CardTag, int>(0);

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

    private IStats _currentStats;
    
    private IStats GetCurrentStats() => _baseStats
        .Plus(_additiveMods.Where(x => x.IsActive).Select(x => x.Stats).ToArray())
        .Times(_multiplierMods.Where(x => x.IsActive).Select(x => x.Stats).ToArray())
        .NotBelowZero()
        .WithWholeNumbersWhereExpected();

    private BattleCounter Counter(string name) => _counters.VerboseGetValue(name, n => $"Counter '{n}'");
    private BattleCounter Counter(StatType statType) => _counters[statType.GetString()];
    private BattleCounter Counter(TemporalStatType statType) => _counters[statType.GetString()];
    public int GetCounterAmount(TemporalStatType statType) => _counters[statType.GetString()].Amount;

    public int MemberId { get; }
    public string NameTerm { get; }

    public MemberState(int id, string name, IStats baseStats, StatType primaryStat)
        : this(id, name, baseStats, primaryStat, baseStats.MaxHp())
    {
    }

    public MemberState(int id, string nameTerm, IStats baseStats, StatType primaryStat, int initialHp)
    {
        MemberId = id;
        NameTerm = nameTerm;
        PrimaryStat = primaryStat;
        _baseStats = baseStats;

        _counters[TemporalStatType.HP.GetString()] =
            new BattleCounter(TemporalStatType.HP, initialHp, () => _currentStats.MaxHp());
        _counters[TemporalStatType.Shield.GetString()] =
            new BattleCounter(TemporalStatType.Shield, Math.Min(baseStats[StatType.StartingShield], _baseStats.MaxShield()), () => _currentStats.MaxShield());
        Enum.GetValues(typeof(TemporalStatType))
            .Cast<TemporalStatType>()
            .Skip(2)
            .ForEach(t => _counters[t.GetString()] = new BattleCounter(t, 0, () => 999));
        _counters[TemporalStatType.Phase.GetString()].Set(1);

        baseStats.ResourceTypes?.ForEach(r =>
            _counters[r.Name] = new BattleCounter(r.Name, Math.Max(0, r.StartingAmount), () => r.MaxAmount));
        if ((baseStats.ResourceTypes?.Length ?? 0) > 0)
            _counters["PrimaryResource"] = _counters[baseStats.ResourceTypes[0].Name];
        
        _counters["None"] = new BattleCounter("None", 0, () => 0);
        _counters[""] = new BattleCounter("", 0, () => 0);
        
        UpdateCurrentStats();
    }

    public void InitResourceAmount(IResourceType resourceType, int amount) => _counters[resourceType.Name].Set(amount);

    // Queries
    public MemberStateSnapshot ToSnapshot()
    {
        var statusTagCounts = new DictionaryWithDefault<StatusTag, int>(0);
        foreach (var tmp in TemporalStates)
        {
            var tag = tmp.Status.Tag;
            if (!statusTagCounts.ContainsKey(tag))
                statusTagCounts[tag] = 0;
            statusTagCounts[tag]++;
        }

        return MemberStateSnapshotExtensions.Create(_versionNumber, MemberId, _currentStats.ToSnapshot(PrimaryStat), BaseStats.ToSnapshot(PrimaryStat),
            _counters.ToDictionary(c => c.Key, c => c.Value.Amount), ResourceTypes, _tagsPlayedCount, statusTagCounts, PrimaryStat);
    }

    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => ResourceAmount(resourceType.Name);
    public int ResourceAmount(string resourceType) => _counters.TryGetValue(resourceType, out var r) ? r.Amount : 0;
    public float this[StatType statType] => statType switch
    {
        StatType.Damagability => _currentStats.Damagability() + (IsVulnerable() ? 0.5f : 0),
        StatType.Healability => _currentStats.Healability() + (IsAntiHeal() ? -0.5f : 0),
        StatType.Power => _currentStats[PrimaryStat],
        _ => _currentStats[statType]
    };
    private bool IsVulnerable() => Counter(TemporalStatType.Vulnerable).Amount > 0;
    private bool IsAntiHeal() => Counter(TemporalStatType.AntiHeal).Amount > 0;
    public float this[TemporalStatType statType] => _counters[statType.GetString()].Amount + _currentStats[statType];
    public IResourceType[] ResourceTypes => _currentStats.ResourceTypes;
    public float Max(string name) => _counters.TryGetValue(name, out var c) ? c.Max : 0;
    public IResourceType PrimaryResource => ResourceTypes.AnyNonAlloc() ? ResourceTypes[0] : new InMemoryResourceType();
    public int PrimaryResourceAmount => ResourceTypes.AnyNonAlloc() ? _counters[PrimaryResource.Name].Amount : 0;

    public ResourceQuantity CurrentPrimaryResources => new ResourceQuantity
        {Amount = PrimaryResourceAmount, ResourceType = PrimaryResource.Name};

    public float PrimaryResourceValue => 1f / 4f;

    public int DifferenceFromBase(StatType statType) => (_currentStats[statType] - _baseStats[statType]).CeilingInt();

    public bool HasStatus(StatusTag tag) => _reactiveStates.AnyNonAlloc(r => r.Status.Tag == tag)
                                            || _additiveMods.AnyNonAlloc(r => r.Status.Tag == tag)
                                            || _multiplierMods.AnyNonAlloc(r => r.Status.Tag == tag);

    public CustomStatusIcon[] CustomStatuses()
        => _customStatusIcons.ToArray();
    
    // Bonus Cards 
    public BonusCardDetails[] GetBonusCards(BattleStateSnapshot snapshot)
        => _bonusCardPlayers
            .Select(x => x.GetBonusCardOnResolutionPhaseBegun(snapshot))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();
    public BonusCardDetails[] GetBonusStartOfTurnCards(BattleStateSnapshot snapshot)
        => _bonusCardPlayers
            .Select(x => x.GetBonusCardOnStartOfTurnPhase(snapshot))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();

    // Reaction Commands
    public ProposedReaction[] GetReactions(EffectResolved e) =>
        ApplicableReactiveStates
            .Where(x => (int)x.Timing > (int)e.Timing)
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
        if (_counters.TryGetValue(statType, out var counter))
            counter.Set(0);
        if (Enum.TryParse(statType, out TemporalStatType tst))
        {
            Log.Info($"Resetting Stat Type {tst}");
            _additiveMods.RemoveAll(m => m.Stats[tst] > 0);
            _multiplierMods.RemoveAll(m => m.Stats[tst] > 1);
        }
        if (Enum.TryParse(statType, out StatType t))
        {
            Log.Info(this[t].GetCeilingIntString());
            Log.Info($"Resetting Stat Type {t}");
            _additiveMods.RemoveAll(m => m.Stats[t] > 0);
            _multiplierMods.RemoveAll(m => m.Stats[t] > 1);
            Log.Info(this[t].GetCeilingIntString());
        }
    });
    
    public void ApplyBonusCardPlayer(IBonusCardPlayer p) => _bonusCardPlayers.Add(p);
    public void ApplyPersistentState(IPersistentState state) => _persistentStates.Add(state);

    public void ApplyTemporaryAdditive(ITemporalState mods) => PublishAfter(() =>
    {
        _additiveMods.Add(mods);
        UpdateCurrentStats();
        if (_currentStats.MaxHp() < _currentStats.Hp())
            SetHp(_currentStats.MaxHp());
    });

    private void UpdateCurrentStats()
    {
        _currentStats = GetCurrentStats().ToSnapshot(PrimaryStat);
    }

    public ITemporalState[] DamageOverTimes() => TemporalStates.Where(x => x.Status.Tag == StatusTag.DamageOverTime).ToArray();

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

    public IEnumerable<ITemporalState> TemporalStates => _additiveMods
        .Concat(_multiplierMods)
        .Concat(_reactiveStates)
        .Concat(_transformers)
        .Concat(_bonusCardPlayers)
        .Concat(_additiveResourceCalculators)
        .Concat(_multiplicativeResourceCalculators);
    
    public int GetNumDebuffs()
    {
        return DebuffStatTypes.Sum(t => _counters[t.GetString()].Amount)
            + _preventedTags.Count 
            + TemporalStates.Count(x => x.IsDebuff);
    }
    
    public int CleanseDebuffs() => -Diff(PublishAfter(() =>
    {
        DebuffStatTypes.ForEach(d => _counters[d.GetString()].Set(0));
        _preventedTags = new Dictionary<CardTag, int>();
        RemoveTemporaryEffects(s => s.IsDebuff);
    }, GetNumDebuffs));

    public void BreakStealth() => PublishAfter(() =>
    {
        RemoveTemporaryEffects(t => t.Status.Tag == StatusTag.Stealth);
        _counters[TemporalStatType.Stealth.GetString()].Set(0);
        _counters[TemporalStatType.Prominent.GetString()].Set(1);
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
    public int TakeTrueDamage(int amount) => -ChangeHp(-amount * this[StatType.Damagability]);
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

        var isDamage = amount < 0;
        if (isDamage)
        {
            Counter(TemporalStatType.OverkillDamageAmount).ChangeBy(-(amount + this[TemporalStatType.HP]));
            Message.Publish(new CharacterAnimationRequested2(MemberId, CharacterAnimationType.WhenHit) { Condition = Maybe<EffectCondition>.Missing() });
        }
        
        Counter(TemporalStatType.HP).ChangeBy(amount);
    }, () => Counter(TemporalStatType.HP).Amount));

    // Status Counters Commands
    public void Adjust(string counterName, float amount) 
        => BattleLog.WriteIf(
            Diff(PublishAfter(() => Counter(counterName).ChangeBy(amount), () => Counter(counterName).Amount)), 
            v => $"{NameTerm.ToEnglish()}'s {counterName} adjusted by {v}", 
            v => v != 0);
    public int Adjust(TemporalStatType t, float amount) => Diff(PublishAfter(() => Counter(t.GetString()).ChangeBy(amount), () => this[t].CeilingInt()));
    public int AdjustShield(float amount) => Adjust(TemporalStatType.Shield, amount);
    private void AdjustShieldNoPublish(float amount) => Counter(TemporalStatType.Shield.GetString()).ChangeBy(amount);

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
    public void ChangeResource(ResourceQuantity qty, PartyAdventureState partyState)
    {
        if (qty.Amount == 0)
            return;
        
        if (qty.Amount < 0)
            LoseResource(qty, partyState, false);
        else
            GainResource(qty, partyState);
    }
    
    public void GainResource(ResourceQuantity qty, PartyAdventureState partyState)
    {
        if (qty.Amount == 0)
            return;
        
        if (qty.ResourceType == "Creds")
            partyState.UpdateCreditsBy(qty.Amount);
        else if (this[TemporalStatType.PreventResourceGains] == 0)
           ChangeResourceAmount(qty, false);
    }
    
    public void AdjustPrimaryResource(int numToGive)
    {
        if (numToGive < 0 || this[TemporalStatType.PreventResourceGains] == 0)
        {
            ChangeResourceAmount(new ResourceQuantity { Amount = numToGive, ResourceType = PrimaryResource.Name }, false);
        }
    }

    public void Spend(ResourceQuantity qty, PartyAdventureState partyState) => LoseResource(qty, partyState, true);
    public void LoseResource(ResourceQuantity qty, PartyAdventureState partyState, bool wasPaidCost)
    {
        if (qty.Amount == 0)
            return;
        
        if (qty.ResourceType == "Creds")
            partyState.UpdateCreditsBy(-qty.Amount);
        else
            ChangeResourceAmount(qty.Negate(), wasPaidCost);
    }

    private void ChangeResourceAmount(ResourceQuantity qty, bool wasPaidCost) => PublishAfter(() =>
    {
        var beforeAmount = Counter(qty.ResourceType).Amount;
        Counter(qty.ResourceType).ChangeBy(qty.Amount);
        var delta = Counter(qty.ResourceType).Amount - beforeAmount;

        if (delta == 0)
            return;
        
        Message.Publish(new MemberResourceChanged(MemberId, qty, wasPaidCost));
        if (wasPaidCost)
            BattleLog.Write($"{NameTerm.ToEnglish()} spent {qty.Negate()}");
        else if (qty.Amount > 0) 
            BattleLog.Write($"{NameTerm.ToEnglish()} gained {qty}");
        else if (qty.Amount < 0)
            BattleLog.Write($"{NameTerm.ToEnglish()} lost {qty.Negate()}");
    });

    public bool HasAnyTemporalStates => _additiveMods.AnyNonAlloc() 
                                        || _multiplierMods.AnyNonAlloc() 
                                        || _reactiveStates.AnyNonAlloc() 
                                        || _persistentStates.AnyNonAlloc() 
                                        || _temporalStatsToReduceAtEndOfTurn.AnyNonAlloc(x => this[x] > 0); 
    
    public IPayloadProvider[] GetTurnStartEffects()
    {
        return _additiveMods.Select(m => m.OnTurnStart())
            .Concat(_multiplierMods.Select(m => m.OnTurnStart()))
            .Concat(_reactiveStates.Select(m => m.OnTurnStart()))
            .Concat(_transformers.Select(m => m.OnTurnStart()))
            .Concat(_additiveResourceCalculators.Select(m => m.OnTurnStart()))
            .Concat(_multiplicativeResourceCalculators.Select(m => m.OnTurnStart()))
            .Concat(_persistentStates.Select(m => m.OnTurnStart()))
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
                + _multiplicativeResourceCalculators.RemoveAll(m => !m.IsActive)
                + _bonusCardPlayers.RemoveAll(m => !m.IsActive);
            if (count > 0)
                DevLog.Write($"Cleaned {count} expired states from {NameTerm.ToEnglish()}");
        });

    private readonly List<TemporalStatType> _temporalStatsToReduceAtEndOfTurn = new List<TemporalStatType>
    {
        TemporalStatType.Taunt,
        TemporalStatType.Stealth, // Should this be reduced?
        TemporalStatType.Confused,
        TemporalStatType.Prominent,
        TemporalStatType.PreventResourceGains,
        TemporalStatType.Vulnerable,
        TemporalStatType.AntiHeal,
    };

    private void WithOtherAdjustmentRulesApplied(MemberStateSnapshot before)
    {
        if (this[TemporalStatType.Stealth] > before[TemporalStatType.Stealth])
            Counter(TemporalStatType.Taunt).Set(0);
        if (before[TemporalStatType.Stealth] > 0 && this[TemporalStatType.Taunt] > before[TemporalStatType.Taunt])
            BreakStealth();
        UpdateCurrentStats();
    }
    
    public IPayloadProvider[] GetTurnEndEffects()
    {
        PublishAfter(() =>
        {
            _temporalStatsToReduceAtEndOfTurn.ForEach(s => _counters[s.GetString()].ChangeBy(-1));
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
            .Concat(_persistentStates.Select(m => m.OnTurnEnd()))
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
