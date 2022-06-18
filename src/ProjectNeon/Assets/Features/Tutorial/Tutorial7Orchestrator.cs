﻿using System.Collections;
using UnityEngine;

public class Tutorial7Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished>
{
    private const string _callerId = "Tutorial7Orchestrator";

    private bool _hasShowedTip;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Shockwave" && !_hasShowedTip)
        {
            Message.Publish(new ShowHeroBattleThought(1, "Tip: top-right corner of a card shows the targeting scope"));
            _hasShowedTip = true;
        }
    }
}