using System.Collections.Generic;
using System.Linq;

public class PartyCorpAffinity : DictionaryWithDefault<Corp, CorpAffinityStrength>
{
    public PartyCorpAffinity()
        : base(CorpAffinityStrength.None)
    {
    }

    public PartyCorpAffinity(IDictionary<Corp, CorpAffinityStrength> values)
        : base(CorpAffinityStrength.None, values)
    {
    }

    public CorpAffinityStrength this[string corpName] => this
        .Where(x => x.Key.Name.Equals(corpName))
        .FirstAsMaybe()
        .Select(x => x.Value, () => CorpAffinityStrength.None);

    public void DevLogInfo()
        => DevLog.Info($"Party Affinity: {string.Join(", ", this.Select(x => $"{x.Key.Name} - {x.Value}"))}");

}
