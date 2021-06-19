using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/All Equipment")]
public class AllEquipment : ScriptableObject
{
    private Dictionary<int, Equipment> _map;
    [UnityEngine.UI.Extensions.ReadOnly] public StaticEquipment[] Equipments; //Unity Collection Readonly

    public Dictionary<int, Equipment> GetMap() => _map ??= Equipments.ToDictionary(x => x.id, x => (Equipment)x);
    public Maybe<Equipment> GetEquipmentById(int id) => GetMap().ValueOrMaybe(id);

    public Maybe<Equipment> GetFromSaveData(GameEquipmentData data)
    {
        if (data.Type == GameEquipmentDataType.GeneratedEquipment)
            return data.GeneratedEquipment;
        if (data.Type == GameEquipmentDataType.StaticEquipmentId)
            return GetEquipmentById(data.StaticEquipmentId);
        Log.Error($"Unknown how to return saved Equipment of type {data.Type}");
        return Maybe<Equipment>.Missing();
    }
}