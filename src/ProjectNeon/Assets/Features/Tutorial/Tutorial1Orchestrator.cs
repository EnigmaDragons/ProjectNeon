using System;
using System.Collections;
using UnityEngine;

public class Tutorial1Orchestrator : OnMessage<StartCardSetupRequested, PlayerCardSelected, CardClicked, CardDragged, CardResolutionStarted>
{
    private const string _callerId = "Tutorial1Orchestrator";
    
    [SerializeField] private float _notClickingCardPromptDelay;
    [SerializeField] private float _notDraggingCardPromptDelay;
    [SerializeField] private float _notTargetingEnemyPromptDelay;

    private bool _hasStarted;
    private bool _hasClickedCard;
    private bool _hasDraggedCard;
    private bool _hasTargetedEnemy;
    private float _timeTilPrompt;
    private bool _firstCardResolved;
    private bool _firstEnemyCardResolved;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.DeckInfo, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.SquadInfo, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyTechPoints, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerResources, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
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
                    Message.Publish(new ShowHeroBattleThought(4, "I knew you would be easy to take. You can't even figure out that you need to click and hold your card"));
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
        StartCoroutine(ShowTutorialAfterDelay());
    }

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(1);
        Message.Publish(new ShowTutorialByName(_callerId));
    }

    protected override void Execute(PlayerCardSelected msg)
    {
        if (!_hasTargetedEnemy)
        {
            _hasTargetedEnemy = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.DeckInfo, true, _callerId));   
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
        if (!_firstCardResolved && msg.Originator == 1)
        {
            _firstCardResolved = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.EnemyInfo, true, _callerId));
        }
        if (!_firstEnemyCardResolved && msg.Originator == 4)
        {
            _firstEnemyCardResolved = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.SquadInfo, true, _callerId));
        }
    }
}