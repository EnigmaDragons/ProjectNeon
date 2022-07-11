using System.Collections;
using UnityEngine;

public class Tutorial9Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>
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
        yield return new WaitForSeconds(1);
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if ((msg.CardName == "Strike" || msg.CardName == "Charged Strike") && !_hasStriked)
        {
            _hasStriked = true;
            Message.Publish(new ShowHeroBattleThought(4, "You are not nearly strong enough to hurt me with those attacks! HAHAHA!"));
        }
        else if (msg.CardName == "True Damage")
        {
            Message.Publish(new ShowHeroBattleThought(4, "Now you see the power of true damage! No armor, resistance, or shields can stop my blade!"));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
}