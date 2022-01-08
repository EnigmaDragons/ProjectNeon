using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="OnlyOnce/EncounterBuilderHistory")]
public class EncounterBuilderHistory : ScriptableObject
{
    [SerializeField] private List<int[]> encounters;

    public List<int[]> Encounters => encounters ??= new List<int[]>();
    
    public void Clear() => Encounters.Clear();

    public void Init(List<int[]> encounterHistory) => encounters = encounterHistory;
    
    public void AddEncounter(int[] enemyIds)
    {
        Encounters.Add(enemyIds);
    }
}
