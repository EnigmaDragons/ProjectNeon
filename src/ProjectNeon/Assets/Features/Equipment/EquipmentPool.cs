using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/EquipmentPool")]
public class EquipmentPool : ScriptableObject
{
    //public so editor script can access
    public StringVariable[] archetypes;
    public Rarity[] includedRarities = new Rarity[] {Rarity.Starter, Rarity.Basic, Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic};
    [UnityEngine.UI.Extensions.ReadOnly] public List<StaticEquipment> all;
    public AllCorps corps;
    [SerializeField] private EquipmentPool[] subPools = new EquipmentPool[0];
    [SerializeField] private int numRandomCommons = 32;
    [SerializeField] private int numRandomUncommons = 16;
    [SerializeField] private int numRandomRares = 6;
    [SerializeField] private int numRandomEpics = 3;
    [SerializeField] private int weaponOdds = 1;
    [SerializeField] private int armorOdds = 1;
    [SerializeField] private int augmentOdds = 3;

    private EquipmentGenerator _generator;
    private EquipmentGenerator Generator => _generator ??= new EquipmentGenerator(corps);
    
    private Dictionary<EquipmentSlot, int> Odds => new Dictionary<EquipmentSlot, int>
    {
        { EquipmentSlot.Weapon, weaponOdds },
        { EquipmentSlot.Armor, armorOdds },
        { EquipmentSlot.Augmentation, augmentOdds }
    };

    public IEnumerable<Equipment> All => all
        .Concat(subPools.SelectMany(s => s.All))
        .Concat(Enumerable.Range(0, numRandomCommons).Select(_ => Generator.GenerateRandomCommon()))
        .Concat(Enumerable.Range(0, numRandomUncommons).Select(_ => Generator.GenerateRandomUncommon()))
        .Concat(Enumerable.Range(0, numRandomRares).Select(_ => Generator.GenerateRandomRare()))
        .Concat(Enumerable.Range(0, numRandomEpics).Select(_ => Generator.GenerateRandomEpic()));

    public IEnumerable<EquipmentSlot> Random(int n)
    {
        var odds = Odds;
        var factoredList = odds.SelectMany(odd => Enumerable.Range(0, odd.Value).Select(_ => odd.Key));
        return Enumerable.Range(0, n).Select(_ => factoredList.Random());
    }
    
    public IEnumerable<Equipment> Random(EquipmentSlot slot, Rarity rarity, HeroCharacter[] party, int n, string corp) => All
        .Where(x => x.Slot == slot && x.Rarity == rarity && party.Any(hero => x.Archetypes.All(hero.Archetypes.Contains) && (string.IsNullOrWhiteSpace(corp) || x.Corp == corp)))
        .DistinctBy(x => x.Description)
        .ToArray()
        .Shuffled()
        .Take(n);

    public Equipment Random(EquipmentSlot slot, Rarity rarity, HeroCharacter[] party)
    {
        try
        {
            return All.Where(x => x.Slot == slot && x.Rarity == rarity && party.Any(hero => x.Archetypes.All(hero.Archetypes.Contains)))
                .Random();
        }
        catch (Exception e)
        {
            Log.Error($"Tried to get Random Equipment: {slot} {rarity} for {string.Join(", ", party.Select(x => x.Name))} and got {e.Message}");
            return All.Random();
        }
    }
}
