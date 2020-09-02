using System.Collections.Generic;
using System.Linq;

public sealed class BattleUnconsciousnessChecker
{
    public List<Member> ProcessUnconsciousMembers(BattleState s)
    {
        var unconscious = s.GetAllNewlyUnconsciousMembers().ToList();
        unconscious.ForEach(m => ResolveUnconsciousMember(m, s));
        return unconscious;
    }

    private void ResolveUnconsciousMember(Member member, BattleState state)
    {
        if (member.TeamType == TeamType.Enemies)
            state.AddRewardCredits(state.GetEnemyById(member.Id).RewardCredits);
        else
            BattleLog.Write($"{member.Name} - {member.Id} is unconscious");
        Message.Publish(new MemberUnconscious(member));
    }
}
