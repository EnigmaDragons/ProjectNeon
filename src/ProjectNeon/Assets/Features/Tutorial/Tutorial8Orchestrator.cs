using System.Collections;
using UnityEngine;

public class Tutorial8Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>, ILocalizeTerms
{
    private const string _callerId = "Tutorial8Orchestrator";

    private bool _hasShowedLine1;
    private bool _hasShowedLine2;
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
        if (msg.CardName != "Energize" && !_hasShowedLine1)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial08-01".ToLocalized()));
            _hasShowedLine1 = true;
        }
        else if (msg.CardName != "Energize" && !_hasShowedLine2)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial08-02".ToLocalized()));
            _hasShowedLine2 = true;
        }
        else if (msg.CardName == "Impale V1" && _hasShowedLine1 && _hasShowedLine2)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial08-03".ToLocalized()));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    public string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/Tutorial08-01",
        "Thoughts/Tutorial08-02",
        "Thoughts/Tutorial08-03",
    };
}