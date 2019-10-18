
// @todo #1:60min Generify this using StatType
public interface IStats
{
    float MaxHP { get; }
    float MaxShield { get; }
    float Attack { get; }
    float Magic { get; }
    float Armor { get; }
    float Resistance { get; }
    IResourceType[] ResourceTypes { get; }
}
