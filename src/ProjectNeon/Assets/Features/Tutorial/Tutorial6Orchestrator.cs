using System.Collections;
using UnityEngine;

public class Tutorial6Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionStarted, BeginTargetSelectionRequested, EndOfTurnStatusEffectsResolved>
{
    private const string _callerId = "Tutorial6Orchestrator";
    
    private bool _firstTurnFinished;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionStarted msg)
    {
        if (msg.Card.Card.Name == "Powered Shield")
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, true, _callerId));
    }

    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        if (msg.Card.Name == "Strike" && !_firstTurnFinished)
            Message.Publish(new ShowHeroBattleThought(5, "Give me your best shot! You can't target anyone else"));
            
    }

    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => _firstTurnFinished = true;
}