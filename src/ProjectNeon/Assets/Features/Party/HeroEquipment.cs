using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroEquipment
{
    [SerializeField] private string[] archetypes;
    [SerializeField] private string weaponName;
    [SerializeField] private string armorName;
    [SerializeField] private string[] augments = new string[3];
    
    private Equipment _weapon;
    private Equipment _armor;
    private readonly Equipment[] _augments = new Equipment[3];

    public HeroEquipment() {}
    public HeroEquipment(params string[] a) => archetypes = a;

    public Maybe<Equipment> Weapon => new Maybe<Equipment>(_weapon);
    public Maybe<Equipment> Armor => new Maybe<Equipment>(_armor);
    public Equipment[] Augments => _augments.Where(a => a?.Archetypes != null).ToArray();
    
    public Equipment[] All => new []{ _weapon, _armor }
        .Concat(_augments)
        .Where(a => a?.Archetypes != null) // Nasty Hack to make this both handle optional serializables and make this work for Standalone Unit Tests
        .ToArray();
    
    public bool CanEquip(Equipment e)
    {
        return e.Archetypes.All(archetypes.Contains);
    }

    public void Unequip(Equipment e)
    {
        if (_weapon == e)
            _weapon = null;
        if (_armor == e)
            _armor = null;
        for (var i = 0; i < _augments.Length; i++)
            if (_augments[i] == e)
            {
                Log.Info($"Unequipped {e.Name} from Augment Slot {i + 1}");
                _augments[i] = null;
                return;
            }
    }

    public void Equip(Equipment e)
    {
        if (!CanEquip(e))
            throw new InvalidOperationException();

        if (e.Slot == EquipmentSlot.Weapon)
        {
            _weapon = e;
            weaponName = _weapon.Name;
        }

        if (e.Slot == EquipmentSlot.Armor)
        {
            _armor = e;
            armorName = _armor.Name;
        }

        if (e.Slot == EquipmentSlot.Augmentation)
            for (var i = 0; i < _augments.Length; i++)
                if (_augments[i] == null)
                {
                    Log.Info($"Equipped {e.Name} to Augment Slot {i + 1}");
                    _augments[i] = e;
                    augments[i] = e.Name;
                    return;
                }
        
    }
}
