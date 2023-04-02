using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroEquipment
{
    private const int MaxAugments = 6;
    
    [SerializeField] private string[] archetypes;
    [SerializeField] private string weaponName;
    [SerializeField] private string armorName;
    [SerializeField] private string[] augments = new string[MaxAugments];
    [SerializeField] private string[] permanents = new string[0];
    [SerializeField] private string[] implants = new string[0];
    
    private StaticEquipment _weapon;
    private StaticEquipment _armor;
    private readonly StaticEquipment[] _augments = new StaticEquipment[MaxAugments];
    private readonly List<StaticEquipment> _permanents = new List<StaticEquipment>();
    private readonly List<InMemoryEquipment> _implants = new List<InMemoryEquipment>();
    
    public HeroEquipment() {}
    public HeroEquipment(params string[] a) => archetypes = a;

    public int TotalSlots => 6;
    public Maybe<StaticEquipment> Weapon => new Maybe<StaticEquipment>(_weapon);
    public Maybe<StaticEquipment> Armor => new Maybe<StaticEquipment>(_armor);
    public StaticEquipment[] Augments => _augments.Where(a => a?.Archetypes != null).ToArray();
    public StaticEquipment[] Permanents => _permanents.ToArray();
    public InMemoryEquipment[] Implants => _implants.ToArray();
    public int OpenSlots => (Weapon.IsMissing ? 1 : 0) + (Armor.IsMissing ? 1 : 0) + _augments.Count(a => a != null); 
    
    public StaticEquipment[] All => new []{ _weapon, _armor }
        .Concat(_augments)
        .Concat(_permanents)
        .Where(a => a?.Archetypes != null) // Nasty Hack to make this both handle optional serializables and make this work for Standalone Unit Tests
        .ToArray();
    
    public bool CanEquip(StaticEquipment e)
    {
        return e.Archetypes.All(archetypes.Contains);
    }

    public bool HasSpareRoomFor(StaticEquipment e)
    {
        if (!CanEquip(e))
            return false;

        if (e.Slot == EquipmentSlot.Weapon)
            return Weapon.IsMissing;
        if (e.Slot == EquipmentSlot.Armor)
            return Armor.IsMissing;
        if (e.Slot == EquipmentSlot.Augmentation)
            return Augments.Length < MaxAugments;
        return true;
    }
    
    public void Unequip(StaticEquipment e)
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

    public void EquipPermanent(StaticEquipment e)
    {
        _permanents.Add(e);
        permanents = _permanents.Select(x => x.Name).ToArray();
    }
    
    public void EquipImplant(InMemoryEquipment e)
    {
        _implants.Add(e);
        implants = _implants.Select(x => x.Name).ToArray();
    }

    public void Equip(StaticEquipment e)
    {
        if (!CanEquip(e))
            throw new InvalidOperationException();

        if (e.Slot == EquipmentSlot.Weapon)
        {
            _weapon = e;
            weaponName = _weapon.Name;
            return;
        }

        if (e.Slot == EquipmentSlot.Armor)
        {
            _armor = e;
            armorName = _armor.Name;
            return;
        }

        var equippedAugment = false;
        if (e.Slot == EquipmentSlot.Augmentation)
            for (var i = 0; i < _augments.Length; i++)
                if (!equippedAugment && _augments[i] == null)
                {
                    Log.Info($"Equipped {e.Name} to Augment Slot {i + 1}");
                    _augments[i] = e;
                    augments[i] = e.Name;
                    equippedAugment = true;
                }
        
        if (!equippedAugment)
            Log.Error($"Did not Successfully Equip Augment {e.Name}. Num Augments Already Equipped {_augments.Length}");
    }
}
