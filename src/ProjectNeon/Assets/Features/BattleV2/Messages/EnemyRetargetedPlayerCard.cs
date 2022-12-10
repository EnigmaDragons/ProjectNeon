public class EnemyRetargetedPlayerCard
{
    public int OriginatorId { get; }
    public int MemberId { get; }

    public EnemyRetargetedPlayerCard(int originatorId, int memberId)
    {
        OriginatorId = originatorId;
        MemberId = memberId;
    }
}