using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/EquipmentSlotIcons")]
public class EquipmentSlotIcons : ScriptableObject
{
    [SerializeField] private EquipmentSlotIcon[] icons;

    public Dictionary<EquipmentSlot, Sprite> All => icons.ToDictionary(e => e.slot, e => e.icon);
}
