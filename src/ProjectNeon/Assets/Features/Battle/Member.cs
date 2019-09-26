
public class Member 
{
    public TeamType TeamType { get; }
    public int hp;
    // @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors for this one so we can set up reactive bindings.
    
    public Member(TeamType team, Stats baseStats)
    {
        TeamType = team;
        hp = baseStats.MaxHP;
    }

}
