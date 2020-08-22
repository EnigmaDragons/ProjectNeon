using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroEquipment
{
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private StaticEquipment weapon;
    [SerializeField] private StaticEquipment armor;
    [SerializeField] private StaticEquipment[] augments = new StaticEquipment[3];

    public HeroEquipment() {}
    public HeroEquipment(CharacterClass c) => characterClass = c; 
    
    public StaticEquipment[] All => new []{ weapon, armor }
        .Concat(augments)
        .Where(a => a?.Classes != null) // Nasty Hack to make this both handle optional serializables and make this work for Standalone Unit Tests
        .ToArray();
    
    public bool CanEquip(StaticEquipment e)
    {
        return e.Classes.Any(c => c.Name.Equals(characterClass.Name));
    }

    public void Unequip(StaticEquipment e)
    {
        if (weapon == e)
            weapon = null;
        if (armor == e)
            armor = null;
        for (var i = 0; i < augments.Length; i++)
            if (augments[i] == e)
                augments[i] = null;
    }

    public void Equip(StaticEquipment e)
    {
        if (!CanEquip(e))
            throw new InvalidOperationException();

        if (e.Slot == EquipmentSlot.Weapon)
            weapon = e;
        if (e.Slot == EquipmentSlot.Armor)
            armor = e;
        if (e.Slot == EquipmentSlot.Augmentation)
            for (var i = 0; i < augments.Length; i++)
                if (augments[i] == null)
                    augments[i] = e;
    }
}
