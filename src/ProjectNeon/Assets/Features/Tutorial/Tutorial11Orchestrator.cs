using System.Collections;
using UnityEngine;

public class Tutorial11Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards>
{
    private const string _callerId = "Tutorial11Orchestrator";

    private bool _hasPlayedAegis;
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
        if (msg.CardName == "Acid Coating")
        {
            Message.Publish(new ShowHeroBattleThought(4, "My Aegis protects me from that horrible acid you like to use"));
        }
        else if (msg.CardName == "Aegis")
        {
            _hasPlayedAegis = true;
            Message.Publish(new ShowHeroBattleThought(4, "You really are a jack of all trades"));
        }
        else if (msg.CardName == "Erase" && !_hasPlayedAegis)
        {
            Message.Publish(new ShowHeroBattleThought(4, "No tricks to save you this time"));
        }
        else if (msg.CardName == "Erase" && _hasPlayedAegis)
        {
            Message.Publish(new ShowHeroBattleThought(4, "A counter to my best move. Fortunately I'm still impervious"));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
}