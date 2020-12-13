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

    private readonly EquipmentGenerator _generator = new EquipmentGenerator();
    
    public IEnumerable<Equipment> All => all
        .Concat(subPools.SelectMany(s => s.All))
        .Concat(Enumerable.Range(0, numRandomCommons).Select(_ => _generator.GenerateRandomCommon()))
        .Concat(Enumerable.Range(0, numRandomUncommons).Select(_ => _generator.GenerateRandomUncommon()))
        .Concat(Enumerable.Range(0, numRandomRares).Select(_ => _generator.GenerateRandomRare()));
}
