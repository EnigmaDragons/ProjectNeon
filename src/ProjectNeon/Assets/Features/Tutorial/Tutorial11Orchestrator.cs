using System.Collections;
using UnityEngine;

public class Tutorial11Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, BattleStateChanged>
{
    private const string _callerId = "Tutorial11Orchestrator";

    private bool _hasPlayedAegis;
    private bool _hasWon;
    private bool _playedAcidCoating;
    private bool _hasShowedOutplayHint;
    private bool _turn2AegisShown;
    private int _turn;
    
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
            _playedAcidCoating = true;
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
        else if (!_hasShowedOutplayHint && _turn == 2 && msg.CardName == "All In" && !_playedAcidCoating)
        {
            _hasShowedOutplayHint = true;
            Message.Publish(new ShowHeroBattleThought(4, "Trying to prevent me from dodging, huh? That sounds like a harmful effect. My Aegis stops that!"));
        }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
    protected override void Execute(BattleStateChanged msg)
    {
        var s = msg.State;
        _turn = s.TurnNumber;
        if (s.Enemies.Length < 1)
            return;
        
        var aegisCount = s.Enemies[0].Member.Aegis();
        if (_turn == 2 && aegisCount > 0 && !_turn2AegisShown)
        {
            _turn2AegisShown = true;
            Message.Publish(new ShowHeroBattleThought(4, "HAHAHA! I still have my Aegis. My victory is guaranteed!"));
        }
    }
}
