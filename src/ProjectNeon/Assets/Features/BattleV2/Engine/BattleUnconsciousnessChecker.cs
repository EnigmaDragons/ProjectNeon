using System.Collections.Generic;
using System.Linq;

public sealed class BattleUnconsciousnessChecker
{
    public List<Member> ProcessRevivedMembers(BattleState s)
    {
        var revived = s.GetAllNewlyRevivedMembers().ToList();
        revived.ForEach(m => ResolveRevivedMember(m, s));
        return revived;
    }
    
    public List<Member> ProcessUnconsciousMembers(BattleState s)
    {
        var unconscious = s.GetAllNewlyUnconsciousMembers().ToList();
        unconscious.ForEach(m => ResolveUnconsciousMember(m, s));
        return unconscious;
    }

    private void ResolveUnconsciousMember(Member member, BattleState state)
    {
        if (member.TeamType == TeamType.Enemies)
        {
            var enemy = state.GetEnemyById(member.Id);
            state.AddRewardCredits(enemy.GetRewardCredits(state.CreditPerPowerLevelRewardFactor));
            state.AddRewardXp(enemy.GetRewardXp(state.XpPerPowerLevelRewardFactor));
        }

        var idString = member.TeamType == TeamType.Enemies ? $" {member.Id} " : " ";
        BattleLog.Write($"{member.Name}{idString}is unconscious");
        Message.Publish(new MemberUnconscious(member));
    }

    private void ResolveRevivedMember(Member member, BattleState state)
    {
        if (member.TeamType == TeamType.Enemies)
        {
            var enemy = state.GetEnemyById(member.Id);
            state.AddRewardCredits(-enemy.GetRewardCredits(state.CreditPerPowerLevelRewardFactor));
            state.AddRewardXp(-enemy.GetRewardXp(state.XpPerPowerLevelRewardFactor));
        }

        var idString = member.TeamType == TeamType.Enemies ? $" {member.Id} " : " ";
        BattleLog.Write($"{member.Name}{idString}is revived");
        Message.Publish(new MemberRevived(member));
    }
}
