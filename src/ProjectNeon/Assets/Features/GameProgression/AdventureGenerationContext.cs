using System.Collections.Generic;
using System.Linq;
using Features.GameProgression;
using UnityEngine;

public class AdventureGenerationContext
{
    public AdventureProgress2 Adventure { get; }
    public AllEnemies Enemies { get; }
    public HashSet<EnemyEncounterDefinition> Encounters { get; }
    public  BossDetails BossDetails { get; }

    public AdventureGenerationContext(AdventureProgress2 adventure, AllEnemies enemies, BossDetails bossDetails)
    {
        Adventure = adventure;
        Enemies = enemies;
        Encounters = new HashSet<EnemyEncounterDefinition>();
        BossDetails = bossDetails;
    }

    public void RecordGeneratedEncounter(IEnumerable<int> enemyIds) => Encounters.Add(new EnemyEncounterDefinition(enemyIds));
    public void RecordGeneratedEncounter(EnemyEncounterDefinition def) => Encounters.Add(def);
}

public class EnemyEncounterDefinition
{
    public int[] EnemyIds { get; }
    public string Key { get; }

    public EnemyEncounterDefinition(IEnumerable<int> enemyIds)
    {
        EnemyIds = enemyIds.OrderBy(x => x).ToArray();
        Key = string.Join(",", EnemyIds);
    }

    public override int GetHashCode() => Key.GetHashCode();
    public override bool Equals(object obj) => obj is EnemyEncounterDefinition def && def.Key == Key;
}
