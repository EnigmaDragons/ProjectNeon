using System.Collections;
using UnityEngine;

public class Tutorial7Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>, ILocalizeTerms
{
    private const string _callerId = "Tutorial7Orchestrator";

    private bool _hasShowedTip;
    private bool _hasWon;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(9);
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Shockwave" && !_hasShowedTip)
        {
            Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial07-01".ToLocalized()));
            _hasShowedTip = true;
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
    public string[] GetLocalizeTerms() => new[] {"Thoughts/Tutorial07-01"};
}