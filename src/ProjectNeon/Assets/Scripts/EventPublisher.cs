using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/EventPublisher")]
public class EventPublisher : ScriptableObject
{
    public void WinBattle() => Message.Publish(new WinBattleWithRewards());
    public void LoseBattle() => Message.Publish(new BattleFinished(TeamType.Enemies));
    public void StartNewGame() => Message.Publish(new StartNewGameRequested(AdventureMode.Standard));
    public void StartNewDraft() => Message.Publish(new StartNewGameRequested(AdventureMode.Draft));
    public void ContinueCurrentGame() => Message.Publish(new ContinueCurrentGame());
    public void StartNextStage() => Message.Publish(new StartNextStage());
    public void ToggleUseCardAsBasic() => Message.Publish(new ToggleUseCardAsBasic());
    public void RecycleCard() => Message.Publish(new RecycleCard());
    public void BeginTurnConfirmation() => Message.Publish(new BeginPlayerTurnConfirmation());
    public void ToggleGameSpeed() => Message.Publish(new ToggleGameSpeed());
    public void ToggleShop() => Message.Publish(new ToggleShop());
    public void ToggleCardShop() => Message.Publish(new ToggleCardShop(false));
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
    public void ActivatePartyDetailsWizardFlow() => Message.Publish(new TogglePartyDetails { ShouldSaveOnFinished = false, ShouldFightOnFinished = true });
    public void ActivateDeckBuilderTutorialFlow() => Message.Publish(new TogglePartyDetails { ShouldSaveOnFinished = false, ShouldFightOnFinished = false });
    public void ToggleCardLibrary() => Message.Publish(new ToggleCardLibrary());
    public void ToggleEnemyCardLibrary() => Message.Publish(new ToggleEnemyCardLibrary());
    public void ToggleGearLibrary() => Message.Publish(new ToggleGearLibrary());
    public void HideDetailedCardView() => Message.Publish(new HideDetailedCardView());
    public void HideDetailedEnemyView() => Message.Publish(new HideEnemyDetails());
    public void GiveAnInjury() => Message.Publish(new UpdatePartyAdventureState(p => p.Heroes[0].Apply(new AdditiveStatInjury { Stat = new StringReference("MaxHp"), Amount = -3 })));
    public void HideHeroLevelUpPathway() => Message.Publish(new HideHeroLevelUpPathway());
    public void GivePartyXp() => Message.Publish(new GivePartyXp(100));
    public void CancelTargetSelection() => Message.Publish(new CancelTargetSelectionRequested());
    public void EnableMapView() => Message.Publish(new ShowNamedTarget("MapView"));
    public void FinishNode() => Message.Publish(new NodeFinished());
    public void ToggleBestiary() => Message.Publish(new ToggleBestiary());
    public void GainRandomEquipment() => Message.Publish(new GainRandomEquipment());
    public void RefreshShop() => Message.Publish(new RefreshShop());
    public void ToggleGameMenu() => Message.Publish(new ToggleGameMenu());
    public void TogglePatchNotes() => Message.Publish(new TogglePatchNotes());
    public void ToggleOptionsMenu() => Message.Publish(new ToggleNamedTarget("OptionsMenu"));
    public void CompleteAnyMapNode() => Message.Publish(new CompleteAnyMapNode());
    public void GuaranteeStoryEvent() => Message.Publish(new GuaranteeStoryEvent());
    public void MoveTutorialNext() => Message.Publish(new TutorialNextRequested());
    public void MoveTutorialPrevious() => Message.Publish(new TutorialPreviousRequested());
    public void ShowTutorialIfNeeded(string tutorialName) => Message.Publish(new ShowTutorialByNameIfNeeded(tutorialName));
    public void ShowTutorial(string tutorialName) => Message.Publish(new ShowTutorialByName(tutorialName));
    public void ApplyGlobalEffect(StaticGlobalEffect e) => Message.Publish(new ApplyGlobalEffect(e));
    public void ClearGlobalEffects() => Message.Publish(new ClearGlobalEffects());
    public void WinGameImmediately() => Message.Publish(new WinGameRequested());
    public void ToggleCredits() => Message.Publish(new ToggleCredits());
    public void ToggleGameProgressionSummary() => Message.Publish(new ToggleNamedTarget("ProgressionView"));
    public void ToggleLearningMenu() => Message.Publish(new ToggleNamedTarget("LearningMenu"));
    public void SkipCutscene() => Message.Publish(new SkipCutsceneRequested());
    public void AdvanceCutscene() => Message.Publish(new AdvanceCutsceneRequested());
    public void HideHeroDetailsView() => Message.Publish(new HideHeroDetailsView());
    public void ToggleNamedUiElement(string elementName) => Message.Publish(new ToggleNamedTarget(elementName));
    public void SaveGame() => Message.Publish(new AutoSaveRequested());
    public void SwitchToDayEnvironment() => Message.Publish(new SetNightMode(false));
    public void SwitchToNightEnvironment() => Message.Publish(new SetNightMode(true));
    public void SkipSegment() => Message.Publish(new SkipSegment());
    public void SetBasicSuperFocusEnabled(bool enabled) => Message.Publish(new SetSuperFocusBasicControl(enabled));
    public void RespawnMap() => Message.Publish(new RespawnMap());
    public void SkipDraftCheat() => Message.Publish(new SkipDraft());
    public void Proceed(string contextName) => Message.Publish(new ProceedRequested(contextName));
    public void DismissUnlockDisplay() => Message.Publish(new DismissUnlockDisplay());
    public void ShowNewscast(Cutscene cutscene) => Message.Publish(new ShowNewscast(cutscene));
    public void CloseUiSubMenus() => Message.Publish(new CloseUiSubMenus());
    public void ShowCurrentTutorialAgain() => Message.Publish(new ShowCurrentTutorialAgain());
}
