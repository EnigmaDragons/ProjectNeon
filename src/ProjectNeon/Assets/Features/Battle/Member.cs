
public class Member 
{
    public string Name { get; }
    public TeamType TeamType { get; }
    public int hp;
    // @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors for this one so we can set up reactive bindings.
    
    public Member(string name, TeamType team, Stats baseStats)
    {
        Name = name;
        TeamType = team;
        hp = baseStats.MaxHP;
    }

}
