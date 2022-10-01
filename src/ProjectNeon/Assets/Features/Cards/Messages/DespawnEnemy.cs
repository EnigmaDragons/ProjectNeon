public class DespawnEnemy
{
    public Member Member { get; }
    public bool PaysRewardCredits { get; }

    public DespawnEnemy(Member member, bool paysRewardCredits)
    {
        Member = member;
        PaysRewardCredits = paysRewardCredits;
    }
}