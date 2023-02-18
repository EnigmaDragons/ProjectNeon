using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProgressionProgress : MonoBehaviour, ILocalizeTerms
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

    public float GetProgressCompletionFactor()
    {
        var progressItems = GetAllProgress();
        var completedFactor = progressItems.Count(x => x.Completed) / (float) progressItems.Length;
        return completedFactor;
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
        return new ProgressionItem(true, $"{"Progressions/Adventure".ToLocalized()} - {tutorialAdventure.MapTitleTerm.ToLocalized()} - {"Heroes/HeroName16".ToLocalized()}", tutorialAdventure.RequiredHeroes.First(), tutorialAdventure, Maybe<Difficulty>.Missing()).AsArray();
    }

    private ProgressionItem[] GetHeroUnlockProgress(BaseHero[] heroes)
    {
        return heroes.Select(x => new ProgressionItem(false, $"{"Progressions/Hero".ToLocalized()} - {"Progressions/Unlocked".ToLocalized()} - {x.NameTerm().ToLocalized()}", x, Maybe<Adventure>.Missing(), Maybe<Difficulty>.Missing())).ToArray();
    }

    private ProgressionItem[] GetAdventureBaseDifficultyProgress(BaseHero[] heroes, Adventure[] adventures, List<AdventureCompletionRecord> records)
    {
        var keyable = records.DistinctBy(x => $"{x.AdventureId}-{x.HeroId}").ToDictionary(x => $"{x.AdventureId}-{x.HeroId}", x => x);
        return adventures
            .SelectMany(a =>
            {
                var adventureWord = (a.Mode == AdventureMode.Draft ? "Progressions/Draft" : "Progressions/Adventure").ToLocalized();
                return heroes
                    .Select(h =>
                        new ProgressionItem(keyable.ContainsKey($"{a.Id}-{h.id}"),
                            $"{adventureWord} - {a.MapTitleTerm.ToLocalized()} - {h.NameTerm().ToLocalized()}", h, a, Maybe<Difficulty>.Missing()));
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
                var adventureWord = (a.Mode == AdventureMode.Draft ? "Progressions/Draft" : "Progressions/Adventure").ToLocalized();
                if (!difficultiesDict.ContainsKey(difficultyId))
                    return;
                
                var difficulty = difficultiesDict[difficultyId];
                items.Add(new ProgressionItem(highestCompleted >= difficultyId, 
                    $"{adventureWord} - {a.MapTitleTerm.ToLocalized()} - {"Progressions/Difficulty".ToLocalized()} - {difficulty.NameTerm.ToLocalized()}", 
                        Maybe<BaseHero>.Missing(), a, difficulty));
            });
        });

        return items.ToArray();
    }

    public string[] GetLocalizeTerms()
        => new[] { "Progressions/Draft", "Progressions/Difficulty", "Progressions/Adventure", "Progressions/Unlocked", "Progressions/Hero", "Progressions/Complete", "Progressions/Incomplete" };
}
