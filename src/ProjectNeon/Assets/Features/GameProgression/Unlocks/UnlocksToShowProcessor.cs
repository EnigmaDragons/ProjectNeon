using System.Linq;
using UnityEngine;

public class UnlocksToShowProcessor : MonoBehaviour
{
    [SerializeField] private Library library;
    [SerializeField] private Adventure tutorialAdventure;

    private void Start()
    {
        var difficulties = library.UnlockedDifficulties.Where(x => x.id > 0);
        var adventures = library.UnlockedAdventures.Except(tutorialAdventure);

        var progressionData = CurrentProgressionData.Data;
        var highestUnlockedDifficulty = progressionData.UnlockedDifficulty;
        var unshownDifficultes = difficulties
            .Where(d => d.id <= highestUnlockedDifficulty && !progressionData.HasShownUnlockForDifficultyId(d.id))
            .OrderBy(d => d.id);

        var unshownAdventureUnlocks =
            adventures.Where(x => !x.IsLocked && !progressionData.HasShownUnlockForAdventure(x.id));

        var allUnlocksToShow = unshownDifficultes.Select(d => new UnlockUiData("New Difficulty", d.Name, d.Image))
            .Concat(unshownAdventureUnlocks.Select(a => new UnlockUiData("New Adventure", a.MapTitle, a.AdventureImage)))
            .ToArray();

        allUnlocksToShow.ForEach(a => Log.Info($"To Show Unlock: {a}"));
    }
}
