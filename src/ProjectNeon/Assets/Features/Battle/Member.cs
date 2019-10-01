public class Member 
{
    public int Id { get; }
    public string Name { get; }
    public string Class { get; }
    public TeamType TeamType { get; }
    public int hp;
    // @todo #54:30min hp property should not be accessed and mutated by other classes. Create accessors for this one so we can set up reactive bindings.
    
    public Member(int id, string name, string characterClass, TeamType team, Stats baseStats)
    {
        Id = id;
        Name = name;
        Class = characterClass;
        TeamType = team;
        hp = baseStats.MaxHP;
    }

}
