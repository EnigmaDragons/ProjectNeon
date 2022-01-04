using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="OnlyOnce/EncounterBuilderHistory")]
public class EncounterBuilderHistory : ScriptableObject
{
    [SerializeField] private List<int[]> encounters;

    public List<int[]> Encounters => encounters;
    
    public void Clear() => encounters.Clear();

    public void Init(List<int[]> encounterHistory) => encounters = encounterHistory;
    
    public void AddEncounter(int[] enemyIds)
    {
        encounters.Add(enemyIds);
    }
}
