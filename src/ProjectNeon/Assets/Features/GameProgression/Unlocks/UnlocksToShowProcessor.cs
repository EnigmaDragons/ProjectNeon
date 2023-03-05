using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlocksToShowProcessor : MonoBehaviour, ILocalizeTerms
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
        
        if (heroes.Count(x => progressionData.RunsFinished >= x.AdventuresPlayedBeforeUnlocked) >= 9)
            Achievements.Record(Achievement.Progress9HeroesUnlocked);
        
        var unshownHeroUnlocks = heroes
            .Where(x => x.AdventuresPlayedBeforeUnlocked > 0 && progressionData.RunsFinished >= x.AdventuresPlayedBeforeUnlocked && !progressionData.HasShownUnlockForHeroId(x.Id));

        var allUnlocksToShow = 
            unshownHeroUnlocks.Take(1).Select(h => new UnlockUiData(ProgressionData.UnlockTypeHero, h.id, "Unlocks/UnlockHeroHeader", h.NameTerm(), h.Bust))
                .Concat(unshownAdventureUnlocks.Take(1).Select(a => new UnlockUiData(ProgressionData.UnlockTypeAdventure, a.id, "Unlocks/UnlockAdventureHeader", a.MapTitleTerm, a.AdventureImage)))
                .Concat(unshownDifficultes.Take(1).Select(d => new UnlockUiData(ProgressionData.UnlockTypeDifficulty, d.id, "Unlocks/UnlockDifficultyHeader", d.NameTerm, d.Image)))
                .ToArray();

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

    public string[] GetLocalizeTerms()
        => new[] { "Unlocks/UnlockAdventureHeader", "Unlocks/UnlockDifficultyHeader", "Unlocks/UnlockHeroHeader" };
}
