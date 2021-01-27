using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hero
{
    [SerializeField] private HeroCharacter character;
    [SerializeField] private HeroHealth health;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;

    public int LevelUpPoints { get; private set; }

    private IStats _levelUpStats = new StatAddends();
    
    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        LevelUpPoints = 0;
        equipment = new HeroEquipment(character.Class);
        health = new HeroHealth(() => Stats);
    }

    public string Name => character.Name;
    public CharacterClass Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => Stats.MaxHp() - health.MissingHp;
    public HeroEquipment Equipment => equipment;
    public HeroSkill[] Skills => character.Skills;
    public HeroHealth Health => health;
    
    // TODO: Maybe don't calculate this every time
    public IStats Stats => Character.Stats
        .Plus(_levelUpStats)
        .Plus(new StatAddends().With(Equipment.All.SelectMany(e => e.ResourceModifiers).ToArray()))
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Plus(health.AdditiveStats)
        .Times(Equipment.All.Select(e => e.MultiplierStats()))
        .Times(health.MultiplicativeStats);

    public void HealToFull() => UpdateState(() => health.HealToFull());
    public void SetHp(int hp) => UpdateState(() => health.SetHp(hp));
    public void AdjustHp(int amount) => UpdateState(() => health.AdjustHp(amount));
    
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => UpdateState(() => equipment.Equip(e));
    public void Unequip(Equipment e) => UpdateState(() => equipment.Unequip(e));
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    public void AdjustLevelUpPoints(int numPoints) => UpdateState(() => LevelUpPoints += numPoints);
    private void AddLevelUpStat(StatAddends stats) => UpdateState(() => _levelUpStats = _levelUpStats.Plus(stats));

    public void ApplyLevelUpPoint(StatAddends stats) =>
        UpdateState(() =>
        {
            AdjustLevelUpPoints(-1);
            AddLevelUpStat(stats);
        });

    // Cleanup Duplication
    public Member AsMemberForTests(int id)
    {
        var m = new Member(id, Character.Name, Character.Class.Name, TeamType.Party, Stats, Character.Class.BattleRole, CurrentHp);
        return WithEquipmentState(m, EffectContext.ForTests(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None));
    }

    public Member AsMember(int id, BattleState state)
    {
        var m = new Member(id, Character.Name, Character.Class.Name, TeamType.Party, Stats, Character.Class.BattleRole, CurrentHp);
        return WithEquipmentState(m,new EffectContext(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.Members, state.PlayerCardZones));
    }

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

    private void UpdateState(Action a)
    {
        a();
        Message.Publish(new HeroStateChanged(this));
    }
}
