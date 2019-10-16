using System.Linq;

public sealed class MemberState : IStats
{
    private readonly IStats _baseStats;
    private IStats _currentStats;
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
        _currentStats = _baseStats;
    }

    public void ApplyTemporary(IStats mods, int numTurns, string effectName)
    {
        // @todo #1:30min Create a design that allows for temporary stat modifications
    }

    public void ApplyUntilEndOfBattle(BattleStats mods)
    {
        mods.Init(_currentStats);
        _currentStats = mods;
    }

    public int MaxHP => _currentStats.MaxHP;
    public int MaxShield => _currentStats.MaxShield;
    public int Attack => _currentStats.Attack;
    public int Magic => _currentStats.Magic;
    public float Armor => _currentStats.Armor;
    public float Resistance => _currentStats.Resistance;
    public IResourceType[] ResourceTypes => _currentStats.ResourceTypes;
    public bool Active(int turn) => true;
}
