using System.Collections.Generic;

public class PartyCorpAffinity : DictionaryWithDefault<string, CorpAffinityStrength>
{
    public PartyCorpAffinity() 
        : base(CorpAffinityStrength.None) { }

    public PartyCorpAffinity(IDictionary<string, CorpAffinityStrength> values) 
        : base(CorpAffinityStrength.None, values) { }
}
