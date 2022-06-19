using System.Collections;
using UnityEngine;

public class Tutorial5Orchestrator : OnMessage<StartCardSetupRequested, BeginTargetSelectionRequested, EndOfTurnStatusEffectsResolved, CardResolutionFinished, WinBattleWithRewards>
{
    private const string _callerId = "Tutorial5Orchestrator";

    [SerializeField] private BattleState battleState;
    
    private bool _firstTurnFinished;
    private bool _hasWon;

    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.PlayerShields, false, _callerId));
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(5);
        if (!_hasWon)
        Message.Publish(new ShowTutorialByName(_callerId));
    }

    protected override void Execute(BeginTargetSelectionRequested msg)
    {
        if (msg.Card.Name == "Strike" && !_firstTurnFinished)
            Message.Publish(new ShowHeroBattleThought(4, "You can't hit me if you can't see me"));
        if (msg.Card.Name == "Hidden Blade" && !battleState.Members[1].IsStealthed())
            Message.Publish(new ShowHeroBattleThought(4, "Hidden blades will do nothing to me while I can SEE you"));
    }

    protected override void Execute(EndOfTurnStatusEffectsResolved msg) => _firstTurnFinished = true;
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Hide" && !_firstTurnFinished)
            Message.Publish(new ShowHeroBattleThought(4, "You have ruined my assassination! Where are you?!"));
        if (msg.CardName == "Hidden Blade" && battleState.Members[4].CurrentHp() == battleState.Members[4].MaxHp())
            Message.Publish(new ShowHeroBattleThought(4, "You can't deal damage with that if you are not in stealth. Foolish foe!")); 
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
}