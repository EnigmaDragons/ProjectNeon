using System.Linq;

public class ApplyStatInjury : Effect
{
    private readonly StatOperation _operation;
    private readonly StringReference _statName;
    private readonly float _amount;

    public ApplyStatInjury(StatOperation operation, StringReference statName, float amount)
    {
        _operation = operation;
        _statName = statName;
        _amount = amount;
    }
    
    public void Apply(EffectContext ctx)
    {
        var heroes = ctx.Target.Members.Where(x => x.TeamType == TeamType.Party);
        heroes
            .SelectMany(x => ctx.AdventureState.Heroes.Where(h => h.Name.Equals(x.Name)))
            .ForEach(ApplyInjury);
    }

    private void ApplyInjury(Hero h)
    {
        if (_operation == StatOperation.Add)
            h.Apply(new AdditiveStatInjury {Stat = _statName, Amount = _amount});
        else
            h.Apply(new MultiplicativeStatInjury {Stat = _statName, Amount = _amount});
    }
}
