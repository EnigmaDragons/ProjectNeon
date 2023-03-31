using System;

[Serializable]
public class EquipmentBox
{
    public StaticEquipment StaticEquipment;
    
    public EquipmentBox() {}

    public EquipmentBox(Equipment equipment)
    {
        Set(equipment);
    }

    public bool IsBoxFilled() => Get() != null;
    
    public Equipment Get()
    {
        if (StaticEquipment != null)
            return StaticEquipment;
        else return null;
    }

    public void Set(Equipment equipment)
    {
        if (equipment == null || equipment is InMemoryEquipment)
            return;
        StaticEquipment = null;
        if (equipment is StaticEquipment staticEquipment)
            StaticEquipment = staticEquipment;
    }
}