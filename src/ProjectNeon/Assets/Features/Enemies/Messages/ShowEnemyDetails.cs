
public class ShowEnemyDetails
{
    public EnemyInstance Enemy { get; }
    public Maybe<Member> Member { get; }

    public ShowEnemyDetails(EnemyInstance e, Maybe<Member> member)
    {
        Enemy = e;
        Member = member;
    } 
}
