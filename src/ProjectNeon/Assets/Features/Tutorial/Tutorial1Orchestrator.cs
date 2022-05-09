using System;
using UnityEngine;

public class Tutorial1Orchestrator : OnMessage<StartCardSetupRequested, PlayerCardSelected, CardClicked, CardDragged, CardResolutionStarted>
{
    [SerializeField] private float _notClickingCardPromptDelay;
    [SerializeField] private float _notDraggingCardPromptDelay;
    [SerializeField] private float _notTargetingEnemyPromptDelay;

    private bool _hasStarted;
    private bool _hasClickedCard;
    private bool _hasDraggedCard;
    private bool _hasTargetedEnemy;
    private float _timeTilPrompt;
    private bool _firstCardResolved;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.DeckInfo, false));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.SquadInfo, false));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false));
        _timeTilPrompt = _notClickingCardPromptDelay;
    }

    private void Update()
    {
        if (_hasStarted && !_hasTargetedEnemy)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                if (!_hasClickedCard)
                {
                    _timeTilPrompt = _notClickingCardPromptDelay;
                    Message.Publish(new ShowHeroBattleThought(4, "I knew you would be easy to take, you can't even figure out you need to click and hold your card"));
                }
                else if (!_hasDraggedCard)
                {
                    _timeTilPrompt = _notDraggingCardPromptDelay;
                    Message.Publish(new ShowHeroBattleThought(4, "You'll never beat me if you don't figure out you need to drag the card you click"));
                }
                else
                {
                    _timeTilPrompt = _notTargetingEnemyPromptDelay;
                    Message.Publish(new ShowHeroBattleThought(4, "Too afraid to actually target me with that card?"));
                }
            }
        }
    }

    protected override void Execute(StartCardSetupRequested msg)
    {
        _hasStarted = true;
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, false));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, false));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyTechPoints, false));
    }

    protected override void Execute(PlayerCardSelected msg)
    {
        if (!_hasTargetedEnemy)
        {
            _hasTargetedEnemy = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.DeckInfo, true));   
        }
    }

    protected override void Execute(CardClicked msg)
    {
        if (!_hasClickedCard)
        {
            _hasClickedCard = true;
            _timeTilPrompt = _notDraggingCardPromptDelay;
        }
    }

    protected override void Execute(CardDragged msg)
    {
        if (!_hasDraggedCard)
        {
            _hasDraggedCard = true;
            _timeTilPrompt = _notTargetingEnemyPromptDelay;
        }
    }

    protected override void Execute(CardResolutionStarted msg)
    {
        if (!_firstCardResolved)
        {
            _firstCardResolved = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, true));
        }
    }
}