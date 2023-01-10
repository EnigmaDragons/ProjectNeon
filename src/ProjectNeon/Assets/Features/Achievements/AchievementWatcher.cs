using UnityEngine;

public class AchievementWatcher : MonoBehaviour
{
    [SerializeField] private BattleState battleState;

    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(OnMemberStateChanged, this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        if (battleState.Members.TryGetValue(msg.MemberId(), out var m) 
                && m.TeamType == TeamType.Enemies 
                && msg.BeforeState.HpAndShield -42 == msg.State.HpAndShield())
            Achievements.Record(Achievement.Combat42Damage);
    }
}
