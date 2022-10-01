using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlocksToShowProcessor : MonoBehaviour
{
    [SerializeField] private Library library;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private UnlockPresenter presenter;

    private Queue<UnlockUiData> _toShow;

    private void Awake() => presenter.Hide();
    
    private void Start()
    {
        var difficulties = library.UnlockedDifficulties.Where(x => x.id > 0);
        var adventures = library.UnlockedAdventures.Except(tutorialAdventure);
        var heroes = library.UnlockedHeroes;

        var progressionData = CurrentProgressionData.Data;
        var highestUnlockedDifficulty = progressionData.UnlockedDifficulty;
        var unshownDifficultes = difficulties
            .Where(d => d.id <= highestUnlockedDifficulty 
                && !progressionData.HasShownUnlockForDifficultyId(d.id))
            .OrderBy(d => d.id);

        var unshownAdventureUnlocks =
            adventures.Where(x => !x.IsLocked 
                && !progressionData.HasShownUnlockForAdventure(x.id));

        var unshownHeroUnlocks = heroes
            .Where(x => x.AdventuresPlayedBeforeUnlocked > 0 && progressionData.RunsFinished >= x.AdventuresPlayedBeforeUnlocked 
                && !progressionData.HasShownUnlockForHeroId(x.Id));

        var allUnlocksToShow = 
            unshownAdventureUnlocks.Select(a => new UnlockUiData(ProgressionData.UnlockTypeAdventure, a.id, "New Unlocked Adventure!", a.MapTitle, a.AdventureImage))
                .Concat(unshownDifficultes.Select(d => new UnlockUiData(ProgressionData.UnlockTypeDifficulty, d.id, "New Unlocked Difficulty!", d.Name, d.Image)))
                .Concat(unshownHeroUnlocks.Select(h => new UnlockUiData(ProgressionData.UnlockTypeHero, h.id, "New Unlocked Hero!", h.Name, h.Bust)))
                .ToArray();

        //allUnlocksToShow.ForEach(a => Log.Info($"To Show Unlock: {a}"));
        _toShow = allUnlocksToShow.ToQueue();
        ShowNext();
    }

    private void ShowNext()
    {
        if (!_toShow.Any())
            return;
        
        this.ExecuteAfterDelay(() =>
        {
            var unlock = _toShow.Dequeue();
            presenter.Show(unlock, () =>
            {
                CurrentProgressionData.Mutate(d => d.Record(unlock.ToDataRecord()));
                ShowNext();
            });
        }, 1f);
    }
}
