using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shopping/EquipmentPool")]
public class EquipmentPool : ScriptableObject
{
    [SerializeField] private List<Equipment> all;

    public IEnumerable<Equipment> All => all;
}
