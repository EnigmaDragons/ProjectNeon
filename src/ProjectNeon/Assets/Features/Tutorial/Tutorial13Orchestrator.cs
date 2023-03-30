using System.Collections;
using UnityEngine;

public class Tutorial13Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, ShowCurrentTutorialAgain>, ILocalizeTerms
{
    private const string _callerId = "Tutorial13Orchestrator";

    private bool _hasCountered;
    private bool _hasBeenStunned;
    private bool _hasWon;

    protected override void Execute(StartCardSetupRequested msg)
    {
        Message.Publish(new PunchYourself(BattleUiElement.ClickableControls));
        StartCoroutine(ShowTutorialAfterDelay());
    }

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
        if (msg.CardName == "Zap" && !_hasBeenStunned)
        {
            _hasBeenStunned = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial13-01".ToLocalized()));
            Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial13-02".ToLocalized()));
        }
        else if (msg.CardName == "Piercing Strike" && !_hasCountered && !_hasBeenStunned)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial13-03".ToLocalized()));
        }
        else if (msg.CardName == "Counter" && !_hasCountered)
        {
            _hasCountered = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial13-04".ToLocalized()));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
    public string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/Tutorial13-01",
        "Thoughts/Tutorial13-02",
        "Thoughts/Tutorial13-03",
        "Thoughts/Tutorial13-04",
    };
}