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
        var progressionData = CurrentProgressionData.Data;
        if (progressionData.RunsFinished == progressionData.ShownUnlocks.Count)
            return;
        
        var difficulties = library.UnlockedDifficulties.Where(x => x.id > 0);
        var adventures = library.UnlockedAdventures.Except(tutorialAdventure);
        var heroes = library.UnlockedHeroes;
        
        var highestUnlockedDifficulty = progressionData.UnlockedDifficulty;
        var unshownDifficultes = difficulties
            .Where(d => d.id <= highestUnlockedDifficulty && !progressionData.HasShownUnlockForDifficultyId(d.id))
            .OrderBy(d => d.id);

        var unshownAdventureUnlocks = adventures
            .Where(x => x.CanBeUnlocked())
            .OrderBy(x => x.UnlockOrder);
        
        if (heroes.Count(x => progressionData.RunsFinished >= x.AdventuresPlayedBeforeUnlocked) >= 9)
            Achievements.Record(Achievement.Progress9HeroesUnlocked);
        
        var unshownHeroUnlocks = heroes
            .Where(x => x.AdventuresPlayedBeforeUnlocked > 0 && progressionData.RunsFinished >= x.AdventuresPlayedBeforeUnlocked && !progressionData.HasShownUnlockForHeroId(x.Id))
            .OrderBy(x => x.AdventuresPlayedBeforeUnlocked);

        if (unshownAdventureUnlocks.Any() && ((unshownHeroUnlocks.None() && unshownDifficultes.None()) || library.UnlockedAdventures.All(x => x.IsLocked || x.IsCompleted)))
            _toShow = unshownAdventureUnlocks.Take(1).Select(a => new UnlockUiData(ProgressionData.UnlockTypeAdventure, a.id, "Unlocks/UnlockAdventureHeader", a.MapTitleTerm, a.AdventureImage)).ToQueue();
        else if (unshownHeroUnlocks.Any() && (unshownDifficultes.None() || heroes
                 .Where(x => x.AdventuresPlayedBeforeUnlocked == 0 || progressionData.HasShownUnlockForHeroId(x.id))
                 .All(h => progressionData.AdventureCompletions.Any(a => a.HeroId == h.id))))
            _toShow = unshownHeroUnlocks.Take(1).Select(h => new UnlockUiData(ProgressionData.UnlockTypeHero, h.id, "Unlocks/UnlockHeroHeader", h.NameTerm(), h.Bust)).ToQueue();
        else if (unshownDifficultes.Any())
            _toShow = unshownDifficultes.Take(1).Select(d => new UnlockUiData(ProgressionData.UnlockTypeDifficulty, d.id, "Unlocks/UnlockDifficultyHeader", d.NameTerm, d.Image)).ToQueue();
        else
            _toShow = new Queue<UnlockUiData>();
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
