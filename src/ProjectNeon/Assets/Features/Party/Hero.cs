using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hero
{
    [SerializeField] private HeroCharacter character;
    [SerializeField] private HeroHealth health;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;
    [SerializeField] private HeroLevels levels;
    [SerializeField] private CardType basicCard;

    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        levels = new HeroLevels();
        equipment = new HeroEquipment(character.Archetypes.ToArray());
        health = new HeroHealth(() => Stats);
        basicCard = character.ClassCard;
    }

    public string Name => character.Name;
    public string Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => Stats.MaxHp() - health.MissingHp;
    public HeroEquipment Equipment => equipment;
    public HeroSkill[] Skills => character.Skills;
    public HeroHealth Health => health;
    public HeroLevels Levels => levels;
    public int Level => levels.CurrentLevel;
    public CardType BasicCard => basicCard;

    public IStats BaseStats => 
        Character.Stats.Plus(levels.LevelUpStats);
    
    // TODO: Maybe don't calculate this every time
    public IStats Stats => Character.Stats
        .Plus(levels.LevelUpStats)
        .Plus(new StatAddends().With(Equipment.All.SelectMany(e => e.ResourceModifiers).ToArray()))
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Plus(health.AdditiveStats)
        .Times(Equipment.All.Select(e => e.MultiplierStats()))
        .Times(health.MultiplicativeStats);

    public void HealToFull() => UpdateState(() => health.HealToFull());
    public void SetHp(int hp) => UpdateState(() => health.SetHp(hp));
    public void AdjustHp(int amount) => UpdateState(() => health.AdjustHp(amount));

    public void SetBasic(CardType c) => basicCard = c;
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => UpdateState(() => equipment.Equip(e));
    public void Unequip(Equipment e) => UpdateState(() => equipment.Unequip(e));
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    public void ApplyPermanent(Equipment e) => UpdateState(() => equipment.EquipPermanent(e));
    
    // Progression
    // [Obsolete("Just use XP instead")] public void LevelUp(int numLevels) => UpdateState(() => levels.LevelUp(numLevels));
    public void AddXp(int xp) => UpdateState(() => levels.AddXp(xp));
    public void ApplyLevelUpPoint(StatAddends stats) => UpdateState(() => levels.ApplyLevelUpStats(stats));

    // Cleanup Duplication
    public Member AsMemberForTests(int id)
    {
        var stats = Stats;
        var m = new Member(id, Character.Name, Character.Class, TeamType.Party, stats, Character.BattleRole, stats.PrimaryStat(Character.Stats), CurrentHp, basicCard);
        return WithEquipmentState(m, EffectContext.ForTests(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, new UnpreventableContext()));
    }

    public Member AsMember(int id)
    {
        var stats = Stats;
        var m = new Member(id, Character.Name, Character.Class, TeamType.Party, stats, Character.BattleRole, stats.PrimaryStat(Character.Stats), CurrentHp, basicCard);
        return m;
    }

    public Member InitEquipmentState(Member m, BattleState state)
    {
        return WithEquipmentState(m,new EffectContext(m, new Single(m), Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.Members, state.PlayerCardZones, new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), state.Party.Credits, state.Party.Credits));
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
