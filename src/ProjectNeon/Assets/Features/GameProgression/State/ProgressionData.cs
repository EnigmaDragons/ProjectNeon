using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ProgressionData
{
    public int[] CompletedAdventureIds = new int[0];
    public List<AdventureCompletionRecord> AdventureCompletions = new List<AdventureCompletionRecord>();

    public bool Completed(int adventureId) => CompletedAdventureIds.Any(a => a == adventureId);
    public bool Completed(int adventureId, int heroId) 
        => AdventureCompletions.Any(a => a.AdventureId == adventureId && a.HeroId == heroId);
    public int UnlockedDifficulty => AdventureCompletions.Any() ? AdventureCompletions.Max(x => x.Difficulty) : 0;

    public void Record(AdventureCompletionRecord r)
    {
        AdventureCompletions.Add(r);
        AdventureCompletions = AdventureCompletions.Distinct().ToList();
    }
}

[Serializable]
public class AdventureCompletionRecord
{
    public int HeroId;
    public int AdventureId;
    public int Difficulty;
    public string Version;

    public bool Matches(AdventureCompletionRecord other) => other.ToString() == ToString();
    public override string ToString() => $"{HeroId}{AdventureId}{Difficulty}{Version}";
    public override int GetHashCode() => ToString().GetHashCode();
}
