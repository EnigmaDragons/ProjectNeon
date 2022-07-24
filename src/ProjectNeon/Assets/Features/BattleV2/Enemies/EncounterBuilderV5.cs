using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EncounterBuilderV5")]
public class EncounterBuilderV5 : ScriptableObject, IEncounterBuilder
{
    public Enemy[] possible;
    public EnemyDissynergy[] dissynergies;
    public int minEnemiesInNormalFight = 2;
    public int maxEnemiesInNormalFight = 4;
    public int minEnemiesInEliteFight = 1;
    public int maxEnemiesInEliteFight = 5;
    public int maxDamageDealers = 5;
    public int maxDamageMitigators = 1;
    public int maxSpecialists = 1;
    public int minNormalEncounterPower = 100;
    public int maxNormalEncounterPower = 2000;
    public int minEliteEncounterPower = 100;
    public int maxEliteEncounterPower = 2000;
    public float flexibility = 0.3f;
    public float randomness = 0.3f;
    [HideInInspector] public PossibleEncounter[] possibleNormalEncounters;
    [HideInInspector] public PossibleEncounter[] possibleEliteEncounters;
    
    public List<EnemyInstance> Generate(int difficulty, int currentChapterNumber, bool isElite)
    {
        var previousFights = CurrentGameData.Data.Fights.Encounters
            .Select(encounterId => possibleNormalEncounters.Concat(possibleEliteEncounters).FirstOrDefault(encounter => encounter.Id == encounterId))
            .Where(x => x != null)
            .ToArray();
        var encounter = new PossibleEncounterChooser().Choose(isElite ? possibleEliteEncounters : possibleNormalEncounters, previousFights, difficulty, flexibility, randomness);
        CurrentGameData.Write(x =>
        {
            x.Fights.Encounters = x.Fights.Encounters.Concat(encounter.Id).ToArray();
            return x;
        });
        return encounter.Enemies.Shuffled().Select(x => x.ForStage(1)).ToList();
    }
}