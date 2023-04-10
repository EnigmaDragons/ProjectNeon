using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PartyEquipmentCollection
{
    private List<StaticEquipment> _all = new List<StaticEquipment>();
    private List<StaticEquipment> _available = new List<StaticEquipment>();
    private List<StaticEquipment> _equipped = new List<StaticEquipment>();
    
    public List<StaticEquipment> All => (_all ??= new List<StaticEquipment>());
    public List<StaticEquipment> Available => (_available ??= new List<StaticEquipment>());
    public List<StaticEquipment> Equipped => (_equipped ??= new List<StaticEquipment>());

    public IEnumerable<StaticEquipment> AvailableFor(BaseHero c) =>
        Available.Where(e => e.Archetypes.All(c.Archetypes.Contains)).ToList();

    public PartyEquipmentCollection(params StaticEquipment[] e) => Add(e);
    
    public void Add(params StaticEquipment[] e)
    {
        if (e == null)
            return;
        
        All.AddRange(e);
        Available.AddRange(e);
    }

    public void MarkEquipped(StaticEquipment e)
    {
        var indexOf = Available.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        Available.RemoveAt(indexOf);
        Equipped.Add(e);
    }

    public void MarkUnequipped(StaticEquipment e)
    {
        var indexOf = Equipped.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        Equipped.RemoveAt(indexOf);
        Available.Add(e);
    }
}
