using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PartyEquipmentCollection
{
    [SerializeField] private List<Equipment> all = new List<Equipment>();

    public List<Equipment> All => all.ToList();
    
    public void Add(Equipment e) => all.Add(e);
}
