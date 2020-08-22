using System;
using UnityEngine;

[Serializable]
public class Hero
{
    [SerializeField] private BaseHero baseHero;
    [SerializeField] private int currentHp;
    [SerializeField] private RuntimeDeck deck;
    [SerializeField] private HeroEquipment equipment;

    public Hero(BaseHero baseHero, RuntimeDeck deck)
    {
        this.baseHero = baseHero;
        this.deck = deck;
        currentHp = baseHero.Stats.MaxHp();
        equipment = new HeroEquipment(baseHero.Class);
    }

    public BaseHero BaseHero => baseHero;
    public RuntimeDeck Deck => deck;
    public int CurrentHp => currentHp;
    public HeroEquipment Equipment => equipment;

    public void HealToFull() => currentHp = baseHero.Stats.MaxHp();
    public void SetHp(int hp) => currentHp = hp;
    public void SetDeck(RuntimeDeck d) => deck = d;
    public void Equip(Equipment e) => equipment.Equip(e);
    public void Unequip(Equipment e) => equipment.Unequip(e);
    public bool CanEquip(Equipment e) => equipment.CanEquip(e);
    
    public Member AsMember(int id) => new Member(id, BaseHero.Name, BaseHero.Class.Name, TeamType.Party, BaseHero.Stats, CurrentHp);
}
