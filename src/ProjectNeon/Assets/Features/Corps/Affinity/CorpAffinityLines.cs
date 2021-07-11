using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class CorpAffinityLines
{
    [SerializeField] private CorpAffinityLevelLines[] levels = new CorpAffinityLevelLines[0];

    public Maybe<string> RandomLine(CorpAffinityStrength strength) 
        => levels == null 
            ? Maybe<string>.Missing() 
            : levels.Where(l => l.AffinityStrength == strength && l.Lines.Length > 0)
                .FirstAsMaybe()
                .Map(x => x.Lines.Random());
}
