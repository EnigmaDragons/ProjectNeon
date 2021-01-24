using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class PartyEquipmentCollection
{
    private List<Equipment> _all = new List<Equipment>();
    private List<Equipment> _available = new List<Equipment>();
    private List<Equipment> _equipped = new List<Equipment>();
    
    public List<Equipment> All => _all.ToList();
    public List<Equipment> Available => _available.ToList();

    public IEnumerable<Equipment> AvailableFor(CharacterClass c) =>
        Available.Where(e => e.Classes.Contains(CharacterClass.All) || e.Classes.Contains(c.Name)).ToList();
    
    public void Add(params Equipment[] e)
    {
        if (e == null)
            return;
        
        _all.AddRange(e);
        _available.AddRange(e);
    }

    public void MarkEquipped(Equipment e)
    {
        var indexOf = _available.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        _available.RemoveAt(indexOf);
        _equipped.Add(e);
    }

    public void MarkUnequipped(Equipment e)
    {
        var indexOf = _equipped.IndexOf(e);
        if (indexOf < 0)
            throw new InvalidOperationException();
        
        _equipped.RemoveAt(indexOf);
        _available.Add(e);
    }
}
