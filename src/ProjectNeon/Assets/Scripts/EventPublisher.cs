using UnityEngine;

[CreateAssetMenu]
public class EventPublisher : ScriptableObject
{
    public void WinBattle() => Message.Publish(new BattleFinished(TeamType.Party));
    public void LoseBattle() => Message.Publish(new BattleFinished(TeamType.Enemies));
}
