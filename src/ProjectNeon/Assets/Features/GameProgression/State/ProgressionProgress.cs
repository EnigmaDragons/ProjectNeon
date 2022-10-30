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
        var isUnlocked = completedAcademy && progress.Completed(firstStoryAdventure.Id);
        return isUnlocked;
    }

    public ProgressionItem[] GetAllProgress()
    {
        var adventureCompletionRecords = CurrentProgressionData.Data.AdventureCompletions;
        var heroes = library.UnlockedHeroes;
        var nonTutorialAdventures = library.UnlockedAdventures.Except(tutorialAdventure).ToArray();
        return GetTutorialProgress()
            .Concat(GetAdventureBaseDifficultyProgress(heroes, nonTutorialAdventures, adventureCompletionRecords))
            .Concat(GetAdventureHigherDifficultyProgress(nonTutorialAdventures, library.UnlockedDifficulties, CurrentProgressionData.Data))
            .ToArray();
    }
    
    private ProgressionItem[] GetTutorialProgress()
    {
        return new ProgressionItem(true, $"Adventure - {tutorialAdventure.MapTitle} - Anon", tutorialAdventure.RequiredHeroes.First(), tutorialAdventure, Maybe<Difficulty>.Missing()).AsArray();
    }

    private ProgressionItem[] GetHeroUnlockProgress(BaseHero[] heroes)
    {
        return heroes.Select(x => new ProgressionItem(false, $"Hero - Unlocked - {x.NameTerm().ToEnglish()}", x, Maybe<Adventure>.Missing(), Maybe<Difficulty>.Missing())).ToArray();
    }

    private ProgressionItem[] GetAdventureBaseDifficultyProgress(BaseHero[] heroes, Adventure[] adventures, List<AdventureCompletionRecord> records)
    {
        var keyable = records.DistinctBy(x => $"{x.AdventureId}-{x.HeroId}").ToDictionary(x => $"{x.AdventureId}-{x.HeroId}", x => x);
        return adventures
            .SelectMany(a =>
            {
                var adventureWord = a.Mode == AdventureMode.Draft ? "Draft" : "Adventure";
                return heroes
                    .Select(h =>
                        new ProgressionItem(keyable.ContainsKey($"{a.Id}-{h.id}"),
                            $"{adventureWord} - {a.MapTitle} - {h.NameTerm().ToEnglish()}", h, a, Maybe<Difficulty>.Missing()));
            })
            .ToArray();
    }
    
    private ProgressionItem[] GetAdventureHigherDifficultyProgress(Adventure[] adventures, Difficulty[] difficulties, ProgressionData data)
    {
        var difficultiesDict = difficulties.ToDictionary(d => d.id, d => d);
        var items = new List<ProgressionItem>();
        adventures.ForEach(a =>
        {
            var highestCompleted = data.HighestCompletedDifficulty(a.id);
            Enumerable.Range(-1, difficulties.Length).ForEach(difficultyId =>
            {
                var adventureWord = a.Mode == AdventureMode.Draft ? "Draft" : "Adventure";
                if (!difficultiesDict.ContainsKey(difficultyId))
                    return;
                
                var difficulty = difficultiesDict[difficultyId];
                items.Add(new ProgressionItem(highestCompleted >= difficultyId, 
                    $"{adventureWord} - {a.MapTitle} - Difficulty - {difficulty.Name}", 
                        Maybe<BaseHero>.Missing(), a, difficulty));
            });
        });

        return items.ToArray();
    }
}
