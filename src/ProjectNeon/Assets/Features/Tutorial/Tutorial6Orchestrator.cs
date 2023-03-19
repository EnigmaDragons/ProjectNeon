using System.Collections;
using UnityEngine;

public class Tutorial6Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionStarted, BeginTargetSelectionRequested, 
    EndOfTurnStatusEffectsResolved, WinBattleWithRewards, ShowCurrentTutorialAgain>, ILocalizeTerms
{
    private const string _callerId = "Tutorial6Orchestrator";
    
    private bool _firstTurnFinished;
    private bool _gainedShields;
    private bool _hasWon;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return TutorialSettings.BattleTutorialPanelPopupDelay;
        ShowTutorial();
    }
    
    protected override void Execute(ShowCurrentTutorialAgain msg) => ShowTutorial();

    private void ShowTutorial()
    {
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }

    
    protected override void Execute(CardResolutionStarted msg)
    {
        if (msg.Card.Card.Name == "Powered Shield" && !_gainedShields)
        {
            _gainedShields = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, true, _callerId));
            Message.Publish(new PunchYourself(BattleUiElement.PlayerShields));
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial06-01".ToLocalized()));
        }
    }

    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        if (msg.Card.Name == "Strike" && !_firstTurnFinished)
            Message.Publish(new ShowHeroBattleThought(5, "Thoughts/Tutorial06-02".ToLocalized()));
            
    }

    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => _firstTurnFinished = true;
    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Thoughts/Tutorial06-01",
            "Thoughts/Tutorial06-02",
        };
}