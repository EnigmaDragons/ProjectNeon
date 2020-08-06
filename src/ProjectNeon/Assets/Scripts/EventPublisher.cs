using UnityEngine;

[CreateAssetMenu]
public class EventPublisher : ScriptableObject
{
    public void WinBattle() => Message.Publish(new BattleFinished(TeamType.Party));
    public void LoseBattle() => Message.Publish(new BattleFinished(TeamType.Enemies));
    public void StartNewGame() => Message.Publish(new StartNewGame());
    public void StartNextStage() => Message.Publish(new StartNextStage());
    public void ToggleUseCardAsBasic() => Message.Publish(new ToggleUseCardAsBasic());
    public void RecycleCard() => Message.Publish(new RecycleCard());
    public void BeginTurnConfirmation() => Message.Publish(new BeginPlayerTurnConfirmation());
}
