using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hero
{
    [SerializeField] private HeroCharacter character;
    [SerializeField] private int missingHp;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;

    public int LevelUpPoints { get; private set; }

    private IStats _levelUpStats = new StatAddends();
    
    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        missingHp = 0;
        LevelUpPoints = 0;
        equipment = new HeroEquipment(character.Class);
    }

    public string Name => character.Name;
    public CharacterClass Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => Stats.MaxHp() - missingHp;
    public HeroEquipment Equipment => equipment;
    public HeroSkill[] Skills => character.Skills;
    
    // TODO: Maybe don't calculate this every time
    public IStats Stats => Character.Stats
        .Plus(_levelUpStats)
        .Plus(new StatAddends().With(Equipment.All.SelectMany(e => e.ResourceModifiers).ToArray()))
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Times(Equipment.All.Select(e => e.MultiplierStats()));

    public void HealToFull() => missingHp = 0;
    public void SetHp(int hp) => missingHp = Stats.MaxHp() - hp;
    public void AdjustHp(int amount) => missingHp = Mathf.Clamp(missingHp - amount, 0, Stats.MaxHp()); 
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => equipment.Equip(e);
    public void Unequip(Equipment e) => equipment.Unequip(e);
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    public void AdjustLevelUpPoints(int numPoints) => LevelUpPoints += numPoints;
    private void AddLevelUpStat(StatAddends stats) => _levelUpStats = _levelUpStats.Plus(stats);

    public void ApplyLevelUpPoint(StatAddends stats)
    {
        AdjustLevelUpPoints(-1);
        AddLevelUpStat(stats);
    }

    public Member AsMember(int id)
    {
        var m = new Member(id, Character.Name, Character.Class.Name, TeamType.Party, Stats, Character.Class.BattleRole, CurrentHp);
        Equipment.All.ForEach(e =>
        {
            m.Apply(s => s.ApplyPersistentState(new EquipmentPersistentState(e, m)));
            e.BattleStartEffects.ForEach(effect => AllEffects.Apply(effect, new EffectContext(m, new Single(m))));
        });
        return m;
    }
}
