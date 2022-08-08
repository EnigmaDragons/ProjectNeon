using System.Collections;
using UnityEngine;

public class Tutorial4Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionStarted, WinBattleWithRewards>
{
    private const string _callerId = "Tutorial4Orchestrator";

    [SerializeField] private BattleState battleState;

    private bool _hasPlayedStatsBuffCard;
    private bool _hasWon;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return TutorialSettings.BattleTutorialPanelPopupDelay;
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }

    protected override void Execute(CardResolutionStarted msg)
    {
        if (_hasPlayedStatsBuffCard)
            return;
        
        if (msg.Originator == 1 && msg.Card.Card.Name == "Strike")
        {
            Message.Publish(new ShowHeroBattleThought(4, "You are not strong enough to penetrate my armor!"));
        }
        else if (msg.Originator == 1 && msg.Card.Card.Name == "Power Up")
        {
            _hasPlayedStatsBuffCard = true;
            Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PrimaryStat, true, _callerId));
            Message.Publish(new PunchYourself(BattleUiElement.PrimaryStat));
            Message.Publish(new ShowHeroBattleThought(1, "My enemy's armor no longer will hold up against my attacks"));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
}