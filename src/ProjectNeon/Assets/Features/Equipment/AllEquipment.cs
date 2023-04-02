using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Equipment")]
public class AllEquipment : ScriptableObject
{
    private Dictionary<int, StaticEquipment> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public StaticEquipment[] Equipments; //Unity Collection Readonly

    public Dictionary<int, StaticEquipment> GetMap() => _map ??= Equipments.ToDictionary(x => x.id, x => x);
    public Maybe<StaticEquipment> GetEquipmentById(int id) => GetMap().ValueOrMaybe(id);

    public Maybe<StaticEquipment> GetFromSaveData(GameEquipmentData data)
    {
        return GetEquipmentById(data.StaticEquipmentId);
    }
}