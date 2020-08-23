using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hero
{
    [SerializeField] private HeroCharacter character;
    [SerializeField] private int currentHp;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;

    public Hero(HeroCharacter character, RuntimeDeck deck)
    {
        this.character = character;
        this.deck = deck;
        currentHp = character.Stats.MaxHp();
        equipment = new HeroEquipment(character.Class);
    }

    public CharacterClass Class => character.Class;
    public HeroCharacter Character => character;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => currentHp;
    public HeroEquipment Equipment => equipment;

    public void HealToFull() => currentHp = character.Stats.MaxHp();
    public void SetHp(int hp) => currentHp = hp;
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => equipment.Equip(e);
    public void Unequip(Equipment e) => equipment.Unequip(e);
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    
    public Member AsMember(int id)
    {
        var stats = Character.Stats
            .Plus(Equipment.All.Select(e => e.AdditiveStats()))
            .Times(Equipment.All.Select(e => e.MultiplierStats()));
        
        var m = new Member(id, Character.Name, Character.Class.Name, TeamType.Party, stats, Character.Class.BattleRole, CurrentHp);
        return m;
    }
}
