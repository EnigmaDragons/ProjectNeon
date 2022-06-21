using System.Collections;
using UnityEngine;

public class Tutorial10Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>
{
    private const string _callerId = "Tutorial10Orchestrator";
    
    private bool _hasStriked;
    private bool _hasAcidCoated;
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
        if (msg.CardName == "Piercing Strike" && !_hasStriked)
        {
            _hasStriked = true;
            Message.Publish(new ShowHeroBattleThought(4, "Sometimes all you need to be immortal is to be unhittable"));
        }
        else if (msg.CardName == "Dodge")
        {
            Message.Publish(new ShowHeroBattleThought(4, "This fight just got interesting..."));
        }
        else if (msg.CardName == "Acid Coating" && _hasAcidCoated)
        {
            _hasAcidCoated = true;
            Message.Publish(new ShowHeroBattleThought(4, "What did you just get ON ME?!"));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
}