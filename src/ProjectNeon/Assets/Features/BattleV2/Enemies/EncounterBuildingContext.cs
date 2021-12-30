using System;
using System.Linq;

public class EncounterBuildingContext
{
    public EnemyInstance[] SelectedEnemies { get; } 
    public EnemyInstance[] PossibleEnemies { get; } 
    public int CurrentDifficulty { get; } 
    public int TargetDifficulty { get; }

    public EncounterBuildingContext(EnemyInstance[] selectedEnemies, EnemyInstance[] possibleEnemies, int currentDifficulty, int targetDifficulty)
    {
        SelectedEnemies = selectedEnemies;
        PossibleEnemies = possibleEnemies;
        CurrentDifficulty = currentDifficulty;
        TargetDifficulty = targetDifficulty;
    }

    public EncounterBuildingContext WithPossibilities(Func<EnemyInstance, bool> condition)
        => WithPossibilities(PossibleEnemies.Where(condition).ToArray());

    public EncounterBuildingContext WithPossibilities(EnemyInstance[] possibleEnemies)
        => new EncounterBuildingContext(SelectedEnemies, possibleEnemies, CurrentDifficulty, TargetDifficulty);

    public EncounterBuildingContext DontAddAnyMoreEnemies() => WithPossibilities(Array.Empty<EnemyInstance>());
}