using System.Linq;

public class ApplyStatInjury : Effect
{
    private readonly StatOperation _operation;
    private readonly StringReference _statName;
    private readonly float _amount;
    private readonly StringReference _injuryName;

    public ApplyStatInjury(StatOperation operation, StringReference statName, float amount, StringReference injuryName)
    {
        _operation = operation;
        _statName = statName;
        _amount = amount;
        _injuryName = injuryName;
    }
    
    public void Apply(EffectContext ctx)
    {
        var heroes = ctx.Target.Members.Where(x => x.TeamType == TeamType.Party).ToList();
        heroes
            .SelectMany(x => ctx.AdventureState.Heroes.Where(h => h.Name.Equals(x.Name)))
            .ForEach(ApplyInjury);
        heroes.ForEach(h => h.State.Adjust(TemporalStatType.Injury, 1));
    }

    private void ApplyInjury(Hero h)
    {
        if (_operation == StatOperation.Add)
            h.Apply(new AdditiveStatInjury {Stat = _statName, Amount = _amount, Name = _injuryName});
        else
            h.Apply(new MultiplicativeStatInjury {Stat = _statName, Amount = _amount, Name = _injuryName});
    }
}
