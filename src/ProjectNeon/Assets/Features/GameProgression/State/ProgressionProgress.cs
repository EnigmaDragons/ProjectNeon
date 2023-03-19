using System;
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
        var nonTutorialAdventures = library.UnlockedAdventures.Where(x => x.IncludeInProgress).Except(tutorialAdventure).ToArray();
        var items = GetProgressItemsV3(heroes, nonTutorialAdventures, adventureCompletionRecords);
        #if UNITY_EDITOR
        Log.Info(string.Join(Environment.NewLine, items.Select(x => x.Description)));
        #endif
        return items;
    }

    // TODO: Localize Description if Needed
    private ProgressionItem[] GetProgressItemsV3(BaseHero[] heroes, Adventure[] nonTutorialAdventures, List<AdventureCompletionRecord> adventureCompletionRecords)
    {
        const int incompleteDifficulty = -99;
        var difficultiesDict = library.UnlockedDifficulties.SafeToDictionary(x => x.id, x => x);
        var heroesDict = heroes.SafeToDictionary(x => x.id, x => x);
        var adventuresDict = nonTutorialAdventures.SafeToDictionary(x => x.id, x => x);
        var bossesDict = library.AllBosses.SafeToDictionary(x => x.id, x => x);
        var highestDifficultyId = incompleteDifficulty;
        var heroDifficulties = new Dictionary<int, int>();
        var bossDifficulties = new Dictionary<int, int>();
        var adventureDifficulties = new Dictionary<int, int>();

        foreach (var h in heroes) 
            heroDifficulties[h.id] = incompleteDifficulty;

        foreach (var a in nonTutorialAdventures)
            adventureDifficulties[a.id] = incompleteDifficulty;

        foreach (var b in bossesDict)
            bossDifficulties[b.Key] = incompleteDifficulty;

        foreach (var r in adventureCompletionRecords)
        {
            var difficulty = r.Difficulty;
            highestDifficultyId = Math.Max(difficulty, highestDifficultyId);
            if (heroDifficulties.ContainsKey(r.HeroId))
                heroDifficulties[r.HeroId] = Math.Max(heroDifficulties[r.HeroId], difficulty);
            if (adventureDifficulties.ContainsKey(r.AdventureId))
            {
                var adventure = adventuresDict[r.AdventureId];
                var adventureProgressId = adventure.SharesProgressId.Select(x => x, () => adventure.id);
                adventureDifficulties[adventureProgressId] = Math.Max(adventureDifficulties[adventureProgressId], difficulty);
            }
            if (bossDifficulties.ContainsKey(r.BossId))
                    bossDifficulties[r.BossId] = Math.Max(bossDifficulties[r.BossId], difficulty);
        }

        var items = new List<ProgressionItem>();
        foreach (var x in heroDifficulties)
        {
            var difficulty = difficultiesDict.ContainsKey(x.Value) ? difficultiesDict[x.Value] : Maybe<Difficulty>.Missing();
            var difficultyName = difficulty.Select(d => d.NameTerm.ToEnglish(), () => "Incomplete");
            var hero = heroesDict[x.Key];
            items.Add(new ProgressionItem(x.Value > incompleteDifficulty, $"Hero - {hero.NameTerm().ToEnglish()} - {difficultyName}", 
                hero, Maybe<Adventure>.Missing(), difficulty, Maybe<Boss>.Missing()));
        } 

        foreach (var x in adventureDifficulties)
        {            
            var difficulty = difficultiesDict.ContainsKey(x.Value) ? difficultiesDict[x.Value] : Maybe<Difficulty>.Missing();
            var difficultyName = difficulty.Select(d => d.NameTerm.ToEnglish(), () => "Incomplete");
            var adventure = adventuresDict[x.Key];
            items.Add(new ProgressionItem(x.Value > incompleteDifficulty, $"Adventure - {adventure.MapTitleTerm.ToLocalized()} - {difficultyName}", 
                Maybe<BaseHero>.Missing(), adventure, difficulty, Maybe<Boss>.Missing())); 
        }

        foreach (var x in bossDifficulties)
        {
            var difficulty = difficultiesDict.ContainsKey(x.Value) ? difficultiesDict[x.Value] : Maybe<Difficulty>.Missing();
            var difficultyName = difficulty.Select(d => d.NameTerm.ToEnglish(), () => "Incomplete");
            var boss = bossesDict[x.Key];
            items.Add(new ProgressionItem(x.Value > incompleteDifficulty, $"Boss - {boss.NameTerm.ToEnglish()} - {difficultyName}", 
                Maybe<BaseHero>.Missing(), Maybe<Adventure>.Missing(), difficulty, boss)); 
        }

        foreach (var x in difficultiesDict)
        {
            var difficulty = x.Value;
            var completedWord = highestDifficultyId >= x.Key ? "Complete" : "Incomplete";
            items.Add(new ProgressionItem(highestDifficultyId >= x.Key, $"Difficulty - {difficulty.NameTerm.ToEnglish()} - {completedWord}", 
                Maybe<BaseHero>.Missing(), Maybe<Adventure>.Missing(), difficulty, Maybe<Boss>.Missing()));
        }

        return items.ToArray();
    }

    #region Progress V2
    private ProgressionItem[] GetProgressItemsV2(BaseHero[] heroes, Adventure[] nonTutorialAdventures, List<AdventureCompletionRecord> adventureCompletionRecords)
    {
        return GetTutorialProgressV2()
            .Concat(GetAdventureBaseDifficultyProgressV2(heroes, nonTutorialAdventures, adventureCompletionRecords))
            .Concat(GetAdventureHigherDifficultyProgressV2(nonTutorialAdventures, library.UnlockedDifficulties, CurrentProgressionData.Data))
            .ToArray();
    }

    private ProgressionItem[] GetTutorialProgressV2()
    {
        return new ProgressionItem(true, $"{"Progressions/Adventure".ToLocalized()} - {tutorialAdventure.MapTitleTerm.ToLocalized()} - {"Heroes/HeroName16".ToLocalized()}", tutorialAdventure.RequiredHeroes.First(), tutorialAdventure, 
            Maybe<Difficulty>.Missing(), Maybe<Boss>.Missing()).AsArray();
    }

    private ProgressionItem[] GetHeroUnlockProgress(BaseHero[] heroes)
    {
        return heroes.Select(x => new ProgressionItem(false, $"{"Progressions/Hero".ToLocalized()} - {"Progressions/Unlocked".ToLocalized()} - {x.NameTerm().ToLocalized()}", x, Maybe<Adventure>.Missing(),
            Maybe<Difficulty>.Missing(), Maybe<Boss>.Missing())).ToArray();
    }

    private ProgressionItem[] GetAdventureBaseDifficultyProgressV2(BaseHero[] heroes, Adventure[] adventures, List<AdventureCompletionRecord> records)
    {
        var keyable = records.DistinctBy(x => $"{x.AdventureId}-{x.HeroId}").ToDictionary(x => $"{x.AdventureId}-{x.HeroId}", x => x);
        return adventures
            .SelectMany(a =>
            {
                var adventureWord = (a.Mode == AdventureMode.Draft ? "Progressions/Draft" : "Progressions/Adventure").ToLocalized();
                return heroes
                    .Select(h =>
                        new ProgressionItem(keyable.ContainsKey($"{a.Id}-{h.id}"),
                            $"{adventureWord} - {a.MapTitleTerm.ToLocalized()} - {h.NameTerm().ToLocalized()}", h, a, Maybe<Difficulty>.Missing(), Maybe<Boss>.Missing()));
            })
            .ToArray();
    }
    
    private ProgressionItem[] GetAdventureHigherDifficultyProgressV2(Adventure[] adventures, Difficulty[] difficulties, ProgressionData data)
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
                        Maybe<BaseHero>.Missing(), a, difficulty, Maybe<Boss>.Missing()));
            });
        });

        return items.ToArray();
    }
    
    #endregion 

    public string[] GetLocalizeTerms()
        => new[] { "Progressions/Draft", "Progressions/Difficulty", "Progressions/Adventure", "Progressions/Unlocked", "Progressions/Hero", "Progressions/Complete", "Progressions/Incomplete" };
}
