using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnlocksToShowProcessor : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private Library library;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private MultiUnlockPresenter presenter;

    private void Awake() => presenter.Hide();
    
    private void Start()
    {
        var progressionData = CurrentProgressionData.Data;
        if (progressionData.RunsFinished == progressionData.UnlocksShown && CurrentProgressionData.Data.HasShownUnlockForAdventure(AdventureIds.OrganizedHarvestorsAdventureId))
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


        var toShow = new List<UnlockUiData>();
        if (unshownAdventureUnlocks.Any())
            toShow.AddRange(unshownAdventureUnlocks.Take(1).Select(a => new UnlockUiData(ProgressionData.UnlockTypeAdventure, a.id, "Unlocks/UnlockAdventureHeader", a.MapTitleTerm, a.AdventureImage)));
        if (unshownHeroUnlocks.Any())
            toShow.AddRange(unshownHeroUnlocks.Take(1).Select(h => new UnlockUiData(ProgressionData.UnlockTypeHero, h.id, "Unlocks/UnlockHeroHeader", h.NameTerm(), h.Bust)));
        if (unshownDifficultes.Any())
            toShow.AddRange(unshownDifficultes.Take(1).Select(d => new UnlockUiData(ProgressionData.UnlockTypeDifficulty, d.id, "Unlocks/UnlockDifficultyHeader", d.NameTerm, d.Image)));
        presenter.Show(toShow.ToArray());
        CurrentProgressionData.Write(x => 
        { 
            toShow.ForEach(unlock => x.Record(new UnlockItemDisplayRecord { ItemId = unlock.ItemId, UnlockType = unlock.UnlockType }));
            x.UnlocksShown = x.UnlocksShown + 1;
            return x;
        });
    }

    public string[] GetLocalizeTerms()
        => new[] { "Unlocks/UnlockAdventureHeader", "Unlocks/UnlockDifficultyHeader", "Unlocks/UnlockHeroHeader" };
}
