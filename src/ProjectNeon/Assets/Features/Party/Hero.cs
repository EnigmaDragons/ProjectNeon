using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class Hero
{
    [SerializeField] private HeroCharacter character;
    [SerializeField] private HeroHealth health;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;
    [SerializeField] private HeroLevels levels;
    [SerializeField] private CardTypeData basicCard;

    private IStats _statAdditions = new StatAddends();
    private Maybe<StatType> _primaryStat = Maybe<StatType>.Missing();
    private IResourceType _primaryResourceType;

    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        levels = new HeroLevels();
        equipment = new HeroEquipment(character.Archetypes.ToArray());
        health = new HeroHealth(() => Stats);
        basicCard = character.BasicCard;
        _primaryResourceType = character.Stats.ResourceTypes.FirstAsMaybe().Select(r => r, () => new InMemoryResourceType());
    }

    public string Name => character.Name;
    public string DisplayName => character.DisplayName();
    public string Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => Stats.MaxHp() - health.MissingHp;
    public HeroEquipment Equipment => equipment;
    public HeroHealth Health => health;
    public HeroLevels Levels => levels;
    public int Level => levels.CurrentLevel;
    public CardTypeData BasicCard => basicCard;
    public StatType PrimaryStat => _primaryStat.OrDefault(NonTemporaryStats.DefaultPrimaryStat(Character.Stats));
    public Maybe<StatType> PlayerPrimaryStatSelection => _primaryStat;
    public HashSet<string> Archetypes => character.Archetypes;

    public IStats BaseStats => 
        Character.Stats.Plus(_statAdditions);
    
    // TODO: Maybe don't calculate this every time
    public IStats NonTemporaryStats => Character.Stats
        .Plus(_statAdditions)
        .Plus(new StatAddends().With(Equipment.All.SelectMany(e =>
            e.ResourceModifiers.Select(r => r.WithPrimaryResourceMappedForOwner(_primaryResourceType))).ToArray()))
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Times(Equipment.All.Select(e => e.MultiplierStats()));

    public IStats Stats => Character.Stats
        .Plus(_statAdditions)
        .Plus(new StatAddends().With(Equipment.All.SelectMany(e => e.ResourceModifiers.Select(r => r.WithPrimaryResourceMappedForOwner(_primaryResourceType))).ToArray()))
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Plus(health.AdditiveStats(PrimaryStat))
        .Times(Equipment.All.Select(e => e.MultiplierStats()))
        .Times(health.MultiplicativeStats(PrimaryStat));

    public IStats PermanentStats => Character.Stats
        .Plus(_statAdditions)
        .Plus(new StatAddends().With(Equipment.Permanents.SelectMany(e => e.ResourceModifiers).ToArray()))
        .Plus(Equipment.Permanents.Select(e => e.AdditiveStats()))
        .Times(Equipment.Permanents.Select(e => e.MultiplierStats()));

    public IStats LevelUpsAndImplants => _statAdditions;

    public void HealToFull() => UpdateState(() => health.HealToFull());
    public void SetHp(int hp) => UpdateState(() => health.SetHp(hp));
    public void AdjustHp(int amount) => UpdateState(() => health.AdjustHp(amount));

    public void SetBasic(CardTypeData c) => basicCard = c;
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void SetLevels(HeroLevels l) => levels = l;
    public void SetHealth(HeroHealth h)
    {
        health = h;
        h.Init(() => Stats);
    }

    public void AddToStats(IStats stats) => UpdateState(() => _statAdditions = _statAdditions.Plus(stats));
    public void Equip(Equipment e) => UpdateState(() => equipment.Equip(e));
    public void Unequip(Equipment e) => UpdateState(() => equipment.Unequip(e));
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    public void ApplyPermanent(Equipment e) => UpdateState(() => equipment.EquipPermanent(e));
    
    // Progression
    public void AddXp(int xp) => UpdateState(() => levels.AddXp(xp));
    public void RecordLevelUpPointSpent(int levelUpOptionId) => UpdateState(() => levels.RecordLevelUpCompleted(levelUpOptionId));

    public Member AsMemberForTests(int id)
    {
        var m = AsMember(id);
        return WithEquipmentState(m, EffectContext.ForTests(m, new Single(m)));
    }

    public Member AsMember(int id) =>
        new Member(id, Character.Name, Character.Class, Character.MaterialType, TeamType.Party, 
            Stats, Character.BattleRole, PrimaryStat, CurrentHp, new Maybe<CardTypeData>(basicCard));

    public Member InitEquipmentState(Member m, BattleState state) 
        => WithEquipmentState(m, CreateEffectContext(m, state));

    public void ApplyBattleEndEquipmentEffects(Member m, BattleState state) 
        => Equipment.All.SelectMany(e => e.BattleEndEffects)
            .ForEach(x => AllEffects.Apply(x, CreateEffectContext(m, state)));

    private EffectContext CreateEffectContext(Member m, BattleState state) => new EffectContext(m, new Single(m),
        Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.RewardState, 
        state.Members, state.PlayerCardZones, new UnpreventableContext(), new SelectionContext(),
        state.AllCards.GetMap(),
        state.Party.Credits, state.Party.Credits, new Dictionary<int, EnemyType>(), () => state.GetNextCardId(),
        new PlayedCardSnapshot[0],
        state.OwnerTints, state.OwnerBusts);

    private Member WithEquipmentState(Member m, EffectContext ctx)
    {
        Equipment.All.ForEach(e =>
        {
            m.Apply(s => s.ApplyPersistentState(new EquipmentPersistentState(e, ctx)));
            e.BattleStartEffects.ForEach(effect => AllEffects.Apply(effect, ctx));
        });
        return m;     
    }

    public void Apply(AdditiveStatInjury injury) => UpdateState(() => health.Apply(injury));
    public void Apply(MultiplicativeStatInjury injury) => UpdateState(() => health.Apply(injury));
    public void HealInjuryByName(string name) => UpdateState(() => health.HealInjuryByName(name));

    public void SetPrimaryStat(StatType stat) => UpdateState(() => _primaryStat = stat);
    public void SetPrimaryStat(Maybe<StatType> stat) => UpdateState(() => _primaryStat = stat);
    
    private void UpdateState(Action a)
    {
        a();
        Message.Publish(new HeroStateChanged(this));
    }
}
