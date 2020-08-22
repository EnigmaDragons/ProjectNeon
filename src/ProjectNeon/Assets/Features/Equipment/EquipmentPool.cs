using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/EquipmentPool")]
public class EquipmentPool : ScriptableObject
{
    [SerializeField] private List<StaticEquipment> all;

    public IEnumerable<StaticEquipment> All => all;
}
