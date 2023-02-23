using System.Collections;
using UnityEngine;

public class Tutorial9Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>, ILocalizeTerms
{
    private const string _callerId = "Tutorial9Orchestrator";

    private bool _hasStriked;
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
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if ((msg.CardName == "Strike" || msg.CardName == "Charged Strike") && !_hasStriked)
        {
            _hasStriked = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial09-01".ToLocalized()));
        }
        else if (msg.CardName == "True Damage")
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial09-02".ToLocalized()));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Thoughts/Tutorial09-01",
            "Thoughts/Tutorial09-02",
        };
}