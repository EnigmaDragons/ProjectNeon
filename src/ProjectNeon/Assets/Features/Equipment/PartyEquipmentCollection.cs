
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PartyEquipmentCollection
{
    [SerializeField] private List<Equipment> All = new List<Equipment>();

    public void Add(Equipment e) => All.Add(e);
}
