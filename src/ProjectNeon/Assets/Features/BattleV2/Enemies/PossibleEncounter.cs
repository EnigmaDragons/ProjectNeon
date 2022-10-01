using System;
using UnityEngine;

[Serializable]
public class PossibleEncounter
{
    public string Id;
    public Enemy[] Enemies;
    public int Power;
    public int MagicDamageDealers;
    public int PhysicalDamageDealers;
    public int DamageDealers;
    public int DamageMitigators;
    public int Specialists;
}