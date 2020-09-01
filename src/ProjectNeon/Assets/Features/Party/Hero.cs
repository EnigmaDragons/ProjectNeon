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

    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        missingHp = 0;
        equipment = new HeroEquipment(character.Class);
    }

    public CharacterClass Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => Stats.MaxHp() - missingHp;
    public HeroEquipment Equipment => equipment;
    
    // TODO: Maybe don't calculate this every time
    public IStats Stats => Character.Stats
        .Plus(Equipment.All.Select(e => e.AdditiveStats()))
        .Times(Equipment.All.Select(e => e.MultiplierStats()));

    public void HealToFull() => missingHp = 0;
    public void SetHp(int hp) => missingHp = Stats.MaxHp() - hp;
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => equipment.Equip(e);
    public void Unequip(Equipment e) => equipment.Unequip(e);
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);

    public Member AsMember(int id)
        => new Member(id, Character.Name, Character.Class.Name, TeamType.Party, Stats, Character.Class.BattleRole, CurrentHp);
}
