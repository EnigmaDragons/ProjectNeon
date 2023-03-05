using System.Collections;
using UnityEngine;

public class Tutorial10Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, ShowCurrentTutorialAgain>, ILocalizeTerms
{
    private const string _callerId = "Tutorial10Orchestrator";
    
    private bool _hasStriked;
    private bool _hasAcidCoated;
    private bool _hasWon;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
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
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Piercing Strike" && !_hasStriked)
        {
            _hasStriked = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial10-01".ToLocalized()));
        }
        else if (msg.CardName == "Dodge")
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial10-02".ToLocalized()));
        }
        else if (msg.CardName == "Acid Coating" && _hasAcidCoated)
        {
            _hasAcidCoated = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial10-03".ToLocalized()));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Thoughts/Tutorial10-01",
            "Thoughts/Tutorial10-02",
            "Thoughts/Tutorial10-03",
        };
}