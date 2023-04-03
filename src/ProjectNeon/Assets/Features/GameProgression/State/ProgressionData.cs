using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ProgressionData
{
    public int[] CompletedAdventureIds = new int[0];
    public List<AdventureCompletionRecord> AdventureCompletions = new List<AdventureCompletionRecord>();
    public List<UnlockItemDisplayRecord> ShownUnlocks = new List<UnlockItemDisplayRecord>();
    public int RunsFinished;
    public int UnlocksShown;
    public bool HasShownWishlistScene;
    public bool HasSeenAlgeronFinalBoss;

    public bool Completed(int adventureId) => CompletedAdventureIds.Any(a => a == adventureId);
    public bool Completed(int adventureId, int heroId) 
        => AdventureCompletions.Any(a => a.AdventureId == adventureId && a.HeroId == heroId);
    
    private const int TutorialAdventureId = 10;
    public IEnumerable<AdventureCompletionRecord> NonTutorialCompletions => AdventureCompletions.Where(a => a.AdventureId != TutorialAdventureId);

    public int UnlockedDifficulty 
        => NonTutorialCompletions.Any() ? NonTutorialCompletions.Max(x => x.Difficulty) + 1 : 0;
    public int HighestCompletedDifficulty(int adventureId) 
        => AdventureCompletions
            .Where(a => a.AdventureId == adventureId)
            .OrderByDescending(a => a.Difficulty)
            .FirstAsMaybe()
            .Select(a => a.Difficulty, () => -99);

    public const string UnlockTypeAdventure = "Adventure";
    public const string UnlockTypeDifficulty = "Difficulty";
    public const string UnlockTypeHero = "Hero";

    public bool HasShownUnlockForAdventure(int adventureId) 
        => ShownUnlocks.Any(s => s.UnlockType == UnlockTypeAdventure && s.ItemId == adventureId);
    
    public bool HasShownUnlockForDifficultyId(int difficultyId) 
        => ShownUnlocks.Any(s => s.UnlockType == UnlockTypeDifficulty && s.ItemId == difficultyId);
    
    public bool HasShownUnlockForHeroId(int heroId)
        => ShownUnlocks.Any(s => s.UnlockType == UnlockTypeHero && s.ItemId == heroId);
    
    public void Record(AdventureCompletionRecord r)
    {
        AdventureCompletions.Add(r);
        AdventureCompletions = AdventureCompletions.Distinct().ToList();
    }

    public void Record(UnlockItemDisplayRecord r)
    {
        ShownUnlocks.Add(r);
        ShownUnlocks = ShownUnlocks.Distinct().ToList();
    }
}

[Serializable]
public class AdventureCompletionRecord
{
    public int HeroId;
    public int AdventureId;
    public int Difficulty;
    public int BossId;
    public string Version;

    public bool Matches(AdventureCompletionRecord other) => other.ToString() == ToString();
    public override string ToString() => $"{HeroId}{AdventureId}{BossId}{Difficulty}{Version}";
    public override int GetHashCode() => ToString().GetHashCode();
}

[Serializable]
public class UnlockItemDisplayRecord
{
    public string UnlockType;
    public int ItemId;
    
    public bool Matches(UnlockItemDisplayRecord other) => other.ToString() == ToString();
    public override string ToString() => $"{UnlockType}{ItemId}";
    public override int GetHashCode() => ToString().GetHashCode();
}
