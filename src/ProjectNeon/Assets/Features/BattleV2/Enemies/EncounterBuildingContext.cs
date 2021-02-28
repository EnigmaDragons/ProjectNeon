using System;
using System.Linq;

public class EncounterBuildingContext
{
    public Enemy[] SelectedEnemies { get; } 
    public Enemy[] PossibleEnemies { get; } 
    public int CurrentDifficulty { get; } 
    public int TargetDifficulty { get; }

    public EncounterBuildingContext(Enemy[] selectedEnemies, Enemy[] possibleEnemies, int currentDifficulty, int targetDifficulty)
    {
        SelectedEnemies = selectedEnemies;
        PossibleEnemies = possibleEnemies;
        CurrentDifficulty = currentDifficulty;
        TargetDifficulty = targetDifficulty;
    }

    public EncounterBuildingContext WithPossibilities(Func<Enemy, bool> condition)
        => WithPossibilities(PossibleEnemies.Where(condition).ToArray());

    public EncounterBuildingContext WithPossibilities(Enemy[] possibleEnemies)
        => new EncounterBuildingContext(SelectedEnemies, possibleEnemies, CurrentDifficulty, TargetDifficulty);
}