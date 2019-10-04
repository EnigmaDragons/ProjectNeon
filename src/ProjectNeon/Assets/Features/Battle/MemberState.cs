using System.Linq;

public sealed class MemberState : IStats
{
    private readonly IStats _baseStats;
    private IStats CurrentStats => _baseStats;
    private int _hp;
    private int _shield;
    private int[] _resources;
    private int[] _resourcesMax;
    
    public int HP
    {
        get => _hp;
        set => _hp = value;
    }

    public int Shield
    {
        get => _shield;
        set => _shield = value;
    }

    public int Resource1 { get; set; }
    public int Resource2 { get; set; }
    
    public MemberState(IStats baseStats)
    {
        _baseStats = baseStats;
        _hp = baseStats.MaxHP;
        _shield = 0;
        _resources = new int[baseStats.ResourceTypes.Length];
        _resourcesMax = baseStats.ResourceTypes.Select(x => x.MaxAmount).ToArray();
    }

    public void ApplyTemporary(IStats mods, int numTurns, string effectName)
    {
        // @todo #1:30min Create a design that allows for temporary stat modifications
    }

    public void ApplyUntilEndOfBattle(IStats mods)
    {
        // @todo #1:30min Create a design that allows for mods that last the whole battle
    }

    public int MaxHP => CurrentStats.MaxHP;
    public int MaxShield => CurrentStats.MaxShield;
    public int Attack => CurrentStats.Attack;
    public int Magic => CurrentStats.Magic;
    public float Armor => CurrentStats.Armor;
    public float Resistance => CurrentStats.Resistance;
    public IResourceType[] ResourceTypes => CurrentStats.ResourceTypes;
}
