public sealed class StatMultipliers : IStats
{
    public float MaxHP { get; set; } = 1f;
    public float MaxShield { get; set; } = 1f;
    public float Attack { get; set; } = 1f;
    public float Magic { get; set; } = 1f;
    public float Armor { get; set; } = 1f;
    public float Resistance { get; set; } = 1f;
    public IResourceType[] ResourceTypes { get; set; } = new IResourceType[0];
}
