using System;
using System.Collections;
using UnityEngine;

public class Tutorial12Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, CardCycled, CardDiscarded>
{
    private const string _callerId = "Tutorial12Orchestrator";
    
    [SerializeField] private BattleState battleState;
    [SerializeField] private float _notCyclingPromptDelay;
    [SerializeField] private float _notDiscardingPromptDelay;

    private int _gilgameshDodges = 9;
    private bool _quickChangePlayed;
    private bool _virusLine;
    private bool _cardCycled;
    private bool _hasWon;
    private int _turn;
    private float _timeTilPrompt;
    private bool _cardDiscarded;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        _timeTilPrompt = _notCyclingPromptDelay;
        _gilgameshDodges = 9;
    }
    
    private void Update()
    {
        if (_turn == 2 && !_cardCycled && _quickChangePlayed)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                _timeTilPrompt = _notCyclingPromptDelay;
                Message.Publish(new ShowHeroBattleThought(1, "I need to cycle my cards by dragging them over the cycle icon at the top of the screen."));
            }
        }
        else if (_turn == 2 && !_cardDiscarded)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                _timeTilPrompt = _notDiscardingPromptDelay;
                Message.Publish(new ShowHeroBattleThought(1, "I can spend actions discarding cards by dragging them over the trash icon at the top of the screen."));
            }
        }
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(9);
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Quick Change" && !_quickChangePlayed)
        {
            _quickChangePlayed = true;
            Message.Publish(new ShowHeroBattleThought(1, "That didn't even cost me an action."));
        }
        else if (_gilgameshDodges > 0 && _gilgameshDodges != battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge))
        {
            var dodgesRemaining = battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge);
            if (dodgesRemaining > 0)
                Message.Publish(new ShowHeroBattleThought(4, $"{dodgesRemaining} dodge{(dodgesRemaining == 1 ? "" : "s")} to go."));
            else
                Message.Publish(new ShowHeroBattleThought(4, $"No more dodges... impressive."));
        }
        else if (msg.CardName == "Virus" && !_virusLine)
        {
            _virusLine = true;
            Message.Publish(new ShowHeroBattleThought(4, "Did I just break all your cards... HAHAHA!"));
        }
        else if (msg.CardName == "Aegis")
        {
            Message.Publish(new ShowHeroBattleThought(4, "Using Aegis to save you from my virus?"));
        }
        if (_gilgameshDodges > 0)
            _gilgameshDodges =  battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge);
        if (_turn == 2 && _cardCycled)
            _timeTilPrompt = _notDiscardingPromptDelay;
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    protected override void Execute(CardCycled msg)
    {
        _timeTilPrompt = _notDiscardingPromptDelay;
        if (!_cardCycled)
        {
            _cardCycled = true;
            Message.Publish(new ShowHeroBattleThought(4, "I can't believe you found a way around my plan!"));
        }
    }

    protected override void Execute(CardDiscarded msg)
    {
        if (!_cardDiscarded)
        {
            _cardDiscarded = true;
            Message.Publish(new ShowHeroBattleThought(4, "Getting rid of dead weight I see."));
        }
    }
}