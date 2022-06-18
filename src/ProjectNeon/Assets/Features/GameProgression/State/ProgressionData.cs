using System;
using System.Linq;

[Serializable]
public class ProgressionData
{
    public int[] CompletedAdventureIds = new int[0];

    public bool Completed(int adventureId) => CompletedAdventureIds.Any(a => a == adventureId);
}
