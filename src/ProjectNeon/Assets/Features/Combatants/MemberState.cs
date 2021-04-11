using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class MemberState : IStats
{
    private int _versionNumber;

    private readonly Dictionary<string, BattleCounter> _counters =
        new Dictionary<string, BattleCounter>(StringComparer.InvariantCultureIgnoreCase);

    private readonly DictionaryWithDefault<CardTag, int> _tagsPlayedCount 
        = new DictionaryWithDefault<CardTag, int>(0);

    private readonly StatType _primaryStat;
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
        _primaryStat = primaryStat;
        _baseStats = baseStats;

        _counters[TemporalStatType.HP.ToString()] =
            new BattleCounter(TemporalStatType.HP, initialHp, () => CurrentStats.MaxHp());
        _counters[TemporalStatType.Shield.ToString()] =
            new BattleCounter(TemporalStatType.Shield, baseStats[StatType.StartingShield], () => CurrentStats.MaxShield());
        Enum.GetValues(typeof(TemporalStatType))
            .Cast<TemporalStatType>()
            .Skip(2)
            .ForEach(t => _counters[t.ToString()] = new BattleCounter(t, 0, () => 999));

        baseStats.ResourceTypes?.ForEach(r =>
            _counters[r.Name] = new BattleCounter(r.Name, r.StartingAmount, () => r.MaxAmount));
        _counters[TemporalStatType.Phase.ToString()].Set(1);
        _counters["None"] = new BattleCounter("None", 0, () => 0);
        _counters[""] = new BattleCounter("", 0, () => 0);
    }

    public void InitResourceAmount(IResourceType resourceType, int amount) => _counters[resourceType.Name].Set(amount);

    // Queries
    public MemberStateSnapshot ToSnapshot()
        => new MemberStateSnapshot(_versionNumber, MemberId, CurrentStats,
            _counters.ToDictionary(c => c.Key, c => c.Value.Amount), ResourceTypes, _tagsPlayedCount);

    public bool IsConscious => this[TemporalStatType.HP] > 0;
    public bool IsUnconscious => !IsConscious;
    public int this[IResourceType resourceType] => _counters[resourceType.Name].Amount;
    public int ResourceAmount(string resourceType) => _counters[resourceType].Amount;
    public float this[StatType statType] => CurrentStats[statType];
    public float this[TemporalStatType statType] => _counters[statType.ToString()].Amount + CurrentStats[statType];
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
    public float Max(string name) => _counters[name].Max;
    public IResourceType PrimaryResource => ResourceTypes.Any() ? ResourceTypes[0] : new InMemoryResourceType();
    public int PrimaryResourceAmount => ResourceTypes.Any() ? _counters[PrimaryResource.Name].Amount : 0;

    public ResourceQuantity CurrentPrimaryResources => new ResourceQuantity
        {Amount = PrimaryResourceAmount, ResourceType = PrimaryResource.Name};

    public float PrimaryResourceValue
    {
        get
        {
            if (PrimaryResource.Name == "Ammo")
                return PrimaryResourceAmount * (1f / 6f);
            if (PrimaryResource.Name == "Chems")
                return PrimaryResourceAmount * (1f / 5f);
            if (PrimaryResource.Name == "Energy")
                return PrimaryResourceAmount * (1f / 3f);
            if (PrimaryResource.Name == "Flames")
                return PrimaryResourceAmount * (1f / 3f);
            return 0f;
        }
    }

    public int DifferenceFromBase(StatType statType) => (CurrentStats[statType] - _baseStats[statType]).CeilingInt();
    public ReactiveStateV2[] ReactiveStates => _reactiveStates.ToArray();

    public bool HasStatus(StatusTag tag) => _reactiveStates.Any(r => r.Status.Tag == tag)
                                            || _additiveMods.Any(r => r.Status.Tag == tag)
                                            || _multiplierMods.Any(r => r.Status.Tag == tag);

    public ITemporalState[] StatusesOfType(StatusTag tag)
        => OfType(_additiveMods, tag)
            .Concat(OfType(_multiplierMods, tag))
            .Concat(OfType(_reactiveStates, tag))
            .Concat(OfType(_bonusCardPlayers, tag))
            .ToArray();

    public CustomStatusIcon[] CustomStatuses()
        => _customStatusIcons.ToArray();

    private IEnumerable<ITemporalState> OfType(IEnumerable<ITemporalState> states, StatusTag tag)
        => states.Where(s => s.Status.Tag == tag);

    // Bonus Cards 
    public CardType[] GetBonusCards(BattleStateSnapshot snapshot)
        => _bonusCardPlayers
            .Select(x => x.GetBonusCardOnResolutionPhaseBegun(snapshot))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();

    // Reaction Commands
    public ProposedReaction[] GetReactions(EffectResolved e) =>
        _reactiveStates
            .Select(x => x.OnEffectResolved(e))
            .Where(x => x.IsPresent)
            .Select(x => x.Value)
            .ToArray();
    
    public EffectData Transform(EffectData effect, EffectContext context)
    {
        foreach (var transformer in _transformers)
            effect = transformer.Modify(effect, context);
        return effect;
    }

    public ResourceCalculations CalculateResources(CardTypeData card)
    {
        var calc = card.CalculateResources(this);
        var additives = _additiveResourceCalculators.Select(x => x.GetModifiers(card, this));
        var multiplicatives = _multiplicativeResourceCalculators.Select(x => x.GetModifiers(card, this));
        return new ResourceCalculations(
            calc.ResourcePaidType,
            (calc.ResourcesPaid + additives.Sum(x => x.ResourcesPaid)) * multiplicatives.Product(x => x.ResourcesPaid),
            calc.ResourceGainedType,
            (calc.ResourcesGained + additives.Sum(x => x.ResourcesGained)) *
            multiplicatives.Product(x => x.ResourcesGained),
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

    // Modifier Commands
    private static readonly HashSet<StatusTag> NonStackingStatuses = new HashSet<StatusTag>
        {StatusTag.Vulnerable, StatusTag.AntiHeal};

    public void DuplicateStatesOfType(StatusTag tag)
    {
        _additiveMods.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(ApplyTemporaryAdditive);
        _multiplierMods.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(ApplyTemporaryMultiplier);
        _reactiveStates.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddReactiveState((ReactiveStateV2) s));
        _transformers.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddEffectTransformer((EffectTransformer) s));
        _additiveResourceCalculators.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddAdditiveResourceCalculator((ResourceCalculator) s));
        _multiplicativeResourceCalculators.Where(s => s.Status.Tag == tag).Select(s => s.CloneOriginal()).ToList()
            .ForEach(s => AddMutliplicativeResourceCalculator((ResourceCalculator) s));
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
        if (NonStackingStatuses.Contains(mods.Status.Tag))
            _additiveMods.RemoveAll(m => m.Status.Tag == mods.Status.Tag);
        _additiveMods.Add(mods);
        if (CurrentStats.MaxHp() < CurrentStats.Hp())
            SetHp(CurrentStats.MaxHp());
    });

    public void ApplyTemporaryMultiplier(ITemporalState mods) => PublishAfter(() =>
    {
        if (NonStackingStatuses.Contains(mods.Status.Tag))
            _multiplierMods.RemoveAll(m => m.Status.Tag == mods.Status.Tag);
        _multiplierMods.Add(mods);
    });

    public void CleanseDebuffs() => PublishAfter(() =>
    {
        _counters[TemporalStatType.Disabled.ToString()].Set(0);
        _counters[TemporalStatType.CardStun.ToString()].Set(0);
        _counters[TemporalStatType.Confused.ToString()].Set(0);
        _counters[TemporalStatType.Blind.ToString()].Set(0);
        _counters[TemporalStatType.Inhibit.ToString()].Set(0);
        _preventedTags = new Dictionary<CardTag, int>();
        RemoveTemporaryEffects(s => s.IsDebuff);
    });

    public void ExitStealth() => PublishAfter(() =>
    {
        RemoveTemporaryEffects(t => t.Status.Tag == StatusTag.Stealth);
        _counters[TemporalStatType.Stealth.ToString()].Set(0);
    });

    public void RemoveTemporaryEffects(Predicate<ITemporalState> condition) => PublishAfter(() =>
    {
        _additiveMods.RemoveAll(condition);
        _multiplierMods.RemoveAll(condition);
        _reactiveStates.RemoveAll(condition);
        _transformers.RemoveAll(condition);
        _multiplicativeResourceCalculators.RemoveAll(condition);
    });
    
    public void AddReactiveState(ReactiveStateV2 state) => PublishAfter(() => _reactiveStates.Add(state));
    public void RemoveReactiveState(ReactiveStateV2 state) => PublishAfter(() => _reactiveStates.Remove(state));
    public void AddEffectTransformer(EffectTransformer transformer) => PublishAfter(() => _transformers.Add(transformer));
    public void AddAdditiveResourceCalculator(ResourceCalculator calculator) => PublishAfter(() => _additiveResourceCalculators.Add(calculator));
    public void AddMutliplicativeResourceCalculator(ResourceCalculator calculator) => PublishAfter(() => _multiplicativeResourceCalculators.Add(calculator));
    public void AddCustomStatus(CustomStatusIcon icon) => PublishAfter(() => _customStatusIcons.Add(icon));

    // HP Commands
    public void GainHp(float amount) => ChangeHp(amount * CurrentStats.Healability());
    public void TakeRawDamage(int amount) => ChangeHp(-amount * CurrentStats.Damagability());
    public void SetHp(float amount) => PublishAfter(() => Counter(TemporalStatType.HP).Set(amount));
    public void TakeDamage(int amount)
    {
        var clampedAmount = Math.Max(amount, 0);
        var shieldModificationAmount = Math.Min(clampedAmount, Counter(TemporalStatType.Shield).Amount);
        clampedAmount -= shieldModificationAmount;
        AdjustShieldNoPublish(-shieldModificationAmount);
        if (clampedAmount > 0)
            ChangeHp(-clampedAmount);
    }
    private void ChangeHp(float amount) => PublishAfter(() => Counter(TemporalStatType.HP).ChangeBy(amount));

    // Status Counters Commands
    public void Adjust(string counterName, float amount) => BattleLog.WriteIf(
        Diff(PublishAfter(() => Counter(counterName).ChangeBy(amount), () => Counter(counterName).Amount)), 
        v => $"{Name}'s {counterName} adjusted by {v}", v => v != 0);
    public int Adjust(TemporalStatType t, float amount) => Diff(PublishAfter(() => Counter(t.ToString()).ChangeBy(amount), () => this[t].CeilingInt()));
    public int AdjustShield(float amount) => Adjust(TemporalStatType.Shield, amount);
    private void AdjustShieldNoPublish(float amount) => Counter(TemporalStatType.Shield.ToString()).ChangeBy(amount);
    public void AdjustEvade(float amount) => Adjust(TemporalStatType.Evade, amount);
    public void AdjustSpellshield(float amount) => Adjust(TemporalStatType.Spellshield, amount);
    public void AdjustDoubleDamage(float amount) => Adjust(TemporalStatType.DoubleDamage, amount);

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
    public void ReducePreventedTagCounters() => _preventedTags.ForEach(x => _preventedTags[x.Key] = x.Value > 0 ? x.Value - 1 : 0);
    
    // Resource Commands
    public void Gain(ResourceQuantity qty) => GainResource(qty.ResourceType, qty.Amount);
    public void GainResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(amount));
    public void AdjustPrimaryResource(int numToGive) => PublishAfter(() => _counters[PrimaryResource.Name].ChangeBy(numToGive));
    public void Lose(ResourceQuantity qty) => LoseResource(qty.ResourceType, qty.Amount);
    public void LoseResource(string resourceName, int amount) => PublishAfter(() => Counter(resourceName).ChangeBy(-amount));
    public void SpendPrimaryResource(int numToGive) => PublishAfter(() => _counters[PrimaryResource.Name].ChangeBy(-numToGive));

    public StatType PrimaryStat => _primaryStat; 

    public bool HasAnyTemporalStates => _additiveMods.Any() || _multiplierMods.Any() || _reactiveStates.Any() || _persistentStates.Any(); 
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
        TemporalStatType.Stealth, 
        TemporalStatType.Confused
    };
    
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

    private T[] PublishAfter<T>(Action action, Func<T> getVal)
    {
        var before = ToSnapshot();
        var beforeVal = getVal();
        _versionNumber++;
        action();
        var afterVal = getVal();
        Message.Publish(new MemberStateChanged(before, this));
        return new T[] {beforeVal, afterVal};
    }
    
    private void PublishAfter(Action action)
    {
        var before = ToSnapshot();
        _versionNumber++;
        action();
        Message.Publish(new MemberStateChanged(before, this));
    }
}
