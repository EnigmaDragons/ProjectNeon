using System.Collections.Generic;

public sealed class MultipliedStats : IStats
{
    private readonly IStats _first;
    private readonly IStats _second;

    public float MaxHP => _first.MaxHP * _second.MaxHP;
    public float MaxShield => _first.MaxShield * _second.MaxShield;
    public float Attack => _first.Attack * _second.Attack;
    public float Magic => _first.Magic * _second.Magic;
    public float Armor => _first.Armor * _second.Armor;
    public float Resistance => _first.Resistance * _second.Resistance;
    
    // @todo #1:15min Combine MaxResource Amounts
    public IResourceType[] ResourceTypes => _first.ResourceTypes;

    public MultipliedStats(IStats first, IStats second)
    {
        _first = first;
        _second = second;
    }
}

public static class MultipliedStatExtensions
{
    public static IStats Times(this IStats first, params IStats[] others) => Times(first, (IEnumerable<IStats>) others);
    public static IStats Times(this IStats first, IEnumerable<IStats> others)
    {
        var result = first;
        foreach (var other in others)
            result = new MultipliedStats(result, other);
        return result;
    }
}
