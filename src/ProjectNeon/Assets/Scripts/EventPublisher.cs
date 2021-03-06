using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/EventPublisher")]
public class EventPublisher : ScriptableObject
{
    public void WinBattle() => Message.Publish(new WinBattleWithRewards());
    public void LoseBattle() => Message.Publish(new BattleFinished(TeamType.Enemies));
    public void StartNewGame() => Message.Publish(new StartNewGame());
    public void StartNextStage() => Message.Publish(new StartNextStage());
    public void ToggleUseCardAsBasic() => Message.Publish(new ToggleUseCardAsBasic());
    public void RecycleCard() => Message.Publish(new RecycleCard());
    public void BeginTurnConfirmation() => Message.Publish(new BeginPlayerTurnConfirmation());
    public void ToggleGameSpeed() => Message.Publish(new ToggleGameSpeed());
    public void ToggleShop() => Message.Publish(new ToggleShop());
    public void ToggleCardShop() => Message.Publish(new ToggleCardShop());
    public void ToggleEquipmentShop() => Message.Publish(new ToggleEquipmentShop());
    public void TogglePartyDetails() => Message.Publish(new TogglePartyDetails());
    public void ShowDeckBuilder() => Message.Publish(new ShowDeckBuilder());
    public void CloseDeckBuilder() => Message.Publish(new CloseDeckBuilder());
    public void GivePlayerTonsOfCredits() => Message.Publish(new GivePartyCredits(1000000));
    public void ToggleBattleLogView() => Message.Publish(new ToggleBattleLogView());
    public void ToggleDevLogView() => Message.Publish(new ToggleDevLogView());
    public void ConfirmSquadSelection() => Message.Publish(new ConfirmSquadSelection());
    public void ToggleClinic() => Message.Publish(new ToggleClinic());
    public void ReducePartyHeroesHpBy10() => Message.Publish(new UpdatePartyAdventureState(p => p.Heroes.ForEach(h => h.AdjustHp(-10))));
    public void ActivatePartyDetailsWizardFlow() => Message.Publish(new TogglePartyDetails { AllowDone = false });
    public void ToggleCardLibrary() => Message.Publish(new ToggleCardLibrary());
    public void ToggleEnemyCardLibrary() => Message.Publish(new ToggleEnemyCardLibrary());
    public void GenerateFreshEquipmentSet() => Message.Publish(new GetFreshEquipmentSet());
    public void HideDetailedCardView() => Message.Publish(new HideDetailedCardView());
    public void HideDetailedEnemyView() => Message.Publish(new HideEnemyDetails());
    public void GiveAnInjury() => Message.Publish(new UpdatePartyAdventureState(p => p.Heroes[0].Apply(new AdditiveStatInjury { Stat = new StringReference("MaxHp"), Amount = -3 })));
}
