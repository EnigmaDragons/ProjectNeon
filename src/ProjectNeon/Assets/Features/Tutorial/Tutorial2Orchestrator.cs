using System;
using UnityEngine;

public class Tutorial2Orchestrator : OnMessage<StartCardSetupRequested, ToggleUseCardAsBasic, CardResolutionStarted, ResolutionsFinished>
{
    private const string _callerId = "Tutorial2Orchestrator";
    
    [SerializeField] private float _notSwappingToBasicPromptDelay;
    
    private float _timeTilPrompt;
    private bool _hasStarted;
    private bool _hasSwappedToBasic;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyTechPoints, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerResources, false, _callerId));
        _timeTilPrompt = _notSwappingToBasicPromptDelay;
    }
    
    private void Update()
    {
        if (_hasStarted && !_hasSwappedToBasic)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                Message.Publish(new ShowHeroBattleThought(4, "It is good you don't know you can right click cards to swap them to your characters basic"));
                _timeTilPrompt = _notSwappingToBasicPromptDelay;
            }
        }
    }
    
    protected override void Execute(StartCardSetupRequested msg) => _hasStarted = true;
    protected override void Execute(ToggleUseCardAsBasic msg) => _hasSwappedToBasic = true;
    protected override void Execute(CardResolutionStarted msg) => Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerResources, true, _callerId));

    protected override void Execute(ResolutionsFinished msg)
    {
        if (msg.Phase == BattleV2Phase.EnemyCards)
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyTechPoints, true, _callerId));
    }
}