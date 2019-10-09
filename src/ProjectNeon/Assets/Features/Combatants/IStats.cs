
public interface IStats
{
    int MaxHP { get; }
    int MaxShield { get; }
    int Attack { get; }
    int Magic { get; }
    float Armor { get; }
    float Resistance { get; }
    IResourceType[] ResourceTypes { get; }
    bool Active(int currentTurn);
}
