public class EnemyInitInformation
{
    public int GameObjectIndex { get; }
    public Enemy Enemy { get; }
    public Member Member { get; }

    public EnemyInitInformation(int gameObjectIndex, Enemy enemy, Member member)
    {
        GameObjectIndex = gameObjectIndex;
        Enemy = enemy;
        Member = member;
    }
}