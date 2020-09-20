using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/EventPublisher")]
public class EventPublisher : ScriptableObject
{
    public void WinBattle() => Message.Publish(new BattleFinished(TeamType.Party));
    public void LoseBattle() => Message.Publish(new BattleFinished(TeamType.Enemies));
    public void StartNewGame() => Message.Publish(new StartNewGame());
    public void StartNextStage() => Message.Publish(new StartNextStage());
    public void ToggleUseCardAsBasic() => Message.Publish(new ToggleUseCardAsBasic());
    public void RecycleCard() => Message.Publish(new RecycleCard());
    public void BeginTurnConfirmation() => Message.Publish(new BeginPlayerTurnConfirmation());
    public void ToggleGameSpeed() => Message.Publish(new ToggleGameSpeed());
    public void ToggleShop() => Message.Publish(new ToggleShop());
    public void TogglePartyDetails() => Message.Publish(new TogglePartyDetails());
    public void ShowDeckBuilder() => Message.Publish(new ShowDeckBuilder());
    public void CloseDeckBuilder() => Message.Publish(new CloseDeckBuilder());
    public void GivePlayerTonsOfCredits() => Message.Publish(new GivePartyCredits(1000000));
    public void LevelUpParty() => Message.Publish(new GrantPartyLevelUp());
    public void ToggleBattleLogView() => Message.Publish(new ToggleBattleLogView());
}
