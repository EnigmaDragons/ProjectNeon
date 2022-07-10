using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionProgress : MonoBehaviour
{
    [SerializeField] private Library library;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private Adventure firstStoryAdventure;
    
    public bool DraftModeIsUnlocked()
    {
        if (firstStoryAdventure == null)
            return false;
        
        var completedAcademy = CurrentAcademyData.Data.IsLicensedBenefactor;
        var progress = CurrentProgressionData.Data;
        return completedAcademy && progress.Completed(firstStoryAdventure.Id);
    }

    public ProgressionItem[] GetAllProgress()
    {
        var adventureCompletionRecords = CurrentProgressionData.Data.AdventureCompletions;
        var heroes = library.UnlockedHeroes;
        var nonTutorialAdventures = library.UnlockedAdventures.Except(tutorialAdventure).ToArray();
        return GetTutorialProgress()
            .Concat(GetAdventureBaseDifficultyProgress(heroes, nonTutorialAdventures, adventureCompletionRecords))
            .Concat(GetAdventureHigherDifficultyProgress(nonTutorialAdventures.Length))
            .ToArray();
    }

    private const int HighestDifficulty = 5;
    
    private ProgressionItem[] GetTutorialProgress()
    {
        return new ProgressionItem(true, $"Adventure - {tutorialAdventure.MapTitle} - Anon").AsArray();
    }

    private ProgressionItem[] GetHeroUnlockProgress(BaseHero[] heroes)
    {
        return heroes.Select(x => new ProgressionItem(false, $"Hero - Unlocked - {x.Name}")).ToArray();
    }

    private ProgressionItem[] GetAdventureBaseDifficultyProgress(BaseHero[] heroes, Adventure[] adventures, List<AdventureCompletionRecord> records)
    {
        var keyable = records.ToDictionary(x => $"{x.AdventureId}-{x.HeroId}", x => x);
        return adventures
            .SelectMany(a =>
            {
                var adventureWord = a.Mode == AdventureMode.Draft ? "Draft" : "Adventure";
                return heroes
                    .Select(h =>
                        new ProgressionItem(keyable.ContainsKey($"{a.Id}-{h.id}"),
                            $"{adventureWord} - {a.MapTitle} - {h.Name}"));
            })
            .ToArray();
    }
    
    private ProgressionItem[] GetAdventureHigherDifficultyProgress(int numAdventures)
    {
        return Enumerable.Range(0, HighestDifficulty * numAdventures)
            .Select(x => new ProgressionItem(false, $"Hidden - Unlockable Later In Early Access")).ToArray();
    }
}
