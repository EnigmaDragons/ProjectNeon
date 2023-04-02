using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PartyEquipmentCollection
{
    private List<StaticEquipment> _all = new List<StaticEquipment>();
    private List<StaticEquipment> _available = new List<StaticEquipment>();
    private List<StaticEquipment> _equipped = new List<StaticEquipment>();
    
    public List<StaticEquipment> All => (_all ?? new List<StaticEquipment>()).ToList();
    public List<StaticEquipment> Available => (_available ?? new List<StaticEquipment>()).ToList();
    public List<StaticEquipment> Equipped => (_equipped ?? new List<StaticEquipment>()).ToList();

    public IEnumerable<StaticEquipment> AvailableFor(BaseHero c) =>
        Available.Where(e => e.Archetypes.All(c.Archetypes.Contains)).ToList();

    public PartyEquipmentCollection(params StaticEquipment[] e) => Add(e);
    
    public void Add(params StaticEquipment[] e)
    {
        if (e == null)
            return;
        
        _all.AddRange(e);
        _available.AddRange(e);
    }

    public void MarkEquipped(StaticEquipment e)
    {
        var indexOf = _available.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        _available.RemoveAt(indexOf);
        _equipped.Add(e);
    }

    public void MarkUnequipped(StaticEquipment e)
    {
        var indexOf = _equipped.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        _equipped.RemoveAt(indexOf);
        _available.Add(e);
    }
}
