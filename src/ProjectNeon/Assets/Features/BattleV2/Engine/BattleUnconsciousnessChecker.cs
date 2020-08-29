public sealed class BattleUnconsciousnessChecker
{
    public void ProcessUnconsciousMembers(BattleState s) 
        => s.GetAllNewlyUnconsciousMembers().ForEach(m => ResolveUnconsciousMember(m, s));

    private void ResolveUnconsciousMember(Member member, BattleState state)
    {
        if (member.TeamType == TeamType.Enemies)
            state.AddRewardCredits(state.GetEnemyById(member.Id).RewardCredits);
        else
            BattleLog.Write($"{member.Name} - {member.Id} is unconscious");
        Message.Publish(new MemberUnconscious(member));
    }
}
