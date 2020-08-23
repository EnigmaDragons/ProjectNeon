using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/EquipmentPool")]
public class EquipmentPool : ScriptableObject
{
    [SerializeField] private List<StaticEquipment> all;
    [SerializeField] private int numRandomCommons = 6;
    [SerializeField] private int numRandomUncommons = 4;

    private readonly EquipmentGenerator _generator = new EquipmentGenerator();
    
    public IEnumerable<Equipment> All => all
        .Concat(Enumerable.Range(0, numRandomCommons).Select(_ => _generator.GenerateRandomCommon()))
        .Concat(Enumerable.Range(0, numRandomUncommons).Select(_ => _generator.GenerateRandomUncommon()));
}
