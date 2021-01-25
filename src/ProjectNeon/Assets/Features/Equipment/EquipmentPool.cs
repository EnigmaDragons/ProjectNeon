using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/EquipmentPool")]
public class EquipmentPool : ScriptableObject
{
    [SerializeField] private List<StaticEquipment> all;
    [SerializeField] private EquipmentPool[] subPools = new EquipmentPool[0];
    [SerializeField] private int numRandomCommons = 32;
    [SerializeField] private int numRandomUncommons = 16;
    [SerializeField] private int numRandomRares = 6;
    [SerializeField] private int weaponOdds = 1;
    [SerializeField] private int armorOdds = 1;
    [SerializeField] private int augmentOdds = 3;

    private readonly EquipmentGenerator _generator = new EquipmentGenerator();

    private Dictionary<EquipmentSlot, int> Odds => new Dictionary<EquipmentSlot, int>
    {
        { EquipmentSlot.Weapon, weaponOdds },
        { EquipmentSlot.Armor, armorOdds },
        { EquipmentSlot.Augmentation, augmentOdds }
    };

    public IEnumerable<Equipment> All => all
        .Concat(subPools.SelectMany(s => s.All))
        .Concat(Enumerable.Range(0, numRandomCommons).Select(_ => _generator.GenerateRandomCommon()))
        .Concat(Enumerable.Range(0, numRandomUncommons).Select(_ => _generator.GenerateRandomUncommon()))
        .Concat(Enumerable.Range(0, numRandomRares).Select(_ => _generator.GenerateRandomRare()));

    public IEnumerable<EquipmentSlot> Random(int n)
    {
        var odds = Odds;
        var factoredList = odds.SelectMany(odd => Enumerable.Range(0, odd.Value).Select(_ => odd.Key));
        return Enumerable.Range(0, n).Select(_ => factoredList.Random());
    }
    
    public IEnumerable<Equipment> Random(EquipmentSlot slot, Rarity rarity, HashSet<string> partyClasses, int n) => All
        .Where(x => x.Slot == slot && x.Rarity == rarity && (x.Classes.None() || x.Classes.Any(partyClasses.Contains)))
        .DistinctBy(x => x.Description)
        .ToArray()
        .Shuffled()
        .Take(n);

    public Equipment Random(EquipmentSlot slot, Rarity rarity, HashSet<string> partyClasses)
    {
        try
        {
            return All.Where(x => x.Slot == slot && x.Rarity == rarity &&
                            (x.Classes.None() || x.Classes.Any(partyClasses.Contains)))
                .Random();
        }
        catch (Exception e)
        {
            Log.Error($"Tried to get Random Equipment: {slot} {rarity} for {string.Join(", ", partyClasses)} and got {e.Message}");
            return All.Random();
        }
    }
}
