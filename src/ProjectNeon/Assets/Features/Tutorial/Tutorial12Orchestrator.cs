using System;
using System.Collections;
using UnityEngine;

public class Tutorial12Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, 
    CardCycled, CardDiscarded, BattleStateChanged, ShowCurrentTutorialAgain>, ILocalizeTerms
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
    private bool _activatedCycleHint;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        _timeTilPrompt = _notCyclingPromptDelay;
        _gilgameshDodges = 6;
    }

    private void Update()
    {
        if (_turn != 2)
            return;
        
        if (!_activatedCycleHint && battleState.PlayerState.CardCycles > 0 && !_cardCycled)
        {
            Log.Info("Activated Cycle Hint");
            _activatedCycleHint = true;
            Message.Publish(new ToggleNamedTarget("CycleHint"));
        }
        else
        {
            Log.Info("Did not activate Cycle Hint");
        }
        
        if (_turn == 2 && !_cardCycled && _quickChangePlayed)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                _timeTilPrompt = _notCyclingPromptDelay;
                if (InputControl.Type == ControlType.Mouse)
                    Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial12-01".ToLocalized()));
                else
                    Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial12-01_Controller".ToLocalized()));
            }
        }
        else if (_turn == 2 && !_cardDiscarded)
        {
            _timeTilPrompt = Math.Max(0, _timeTilPrompt - Time.deltaTime);
            if (_timeTilPrompt <= 0)
            {
                _timeTilPrompt = _notDiscardingPromptDelay;
                if (InputControl.Type == ControlType.Mouse)
                    Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial12-02".ToLocalized()));
                else
                    Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial12-02_Controller".ToLocalized()));
            }
        }
    }
    
    protected override void Execute(StartCardSetupRequested msg) => StartCoroutine(ShowTutorialAfterDelay());

    private IEnumerator ShowTutorialAfterDelay()
    {
        yield return new WaitForSeconds(9);
        ShowTutorial();
    }
    
    protected override void Execute(ShowCurrentTutorialAgain msg) => ShowTutorial();

    private void ShowTutorial()
    {
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName == "Quick Change" && !_quickChangePlayed)
        {
            _quickChangePlayed = true;
            Message.Publish(new ShowHeroBattleThought(1, "Thoughts/Tutorial12-03".ToLocalized()));
        }
        else if (_gilgameshDodges > 0 && _gilgameshDodges != battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge))
        {
            var dodgesRemaining = battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge);
            if (dodgesRemaining > 0)
                Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-04".ToLocalized().SafeFormatWithDefault("{0} dodge to go.", dodgesRemaining)));
            else
                Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-05".ToLocalized()));
        }
        else if (msg.CardName == "Virus" && !_virusLine)
        {
            _virusLine = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-06".ToLocalized()));
        }
        else if (msg.CardName == "Aegis")
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-07".ToLocalized()));
        }
        if (_gilgameshDodges > 0)
            _gilgameshDodges =  battleState.Members[4].State.GetCounterAmount(TemporalStatType.Dodge);
        if (_turn == 2 && _cardCycled)
            _timeTilPrompt = _notDiscardingPromptDelay;
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    protected override void Execute(CardCycled msg)
    {
        if (_activatedCycleHint)
            Message.Publish(new HideNamedTarget("CycleHint"));
        _timeTilPrompt = _notDiscardingPromptDelay;
        if (!_cardCycled)
        {
            _cardCycled = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-08".ToLocalized()));
        }
    }

    protected override void Execute(CardDiscarded msg)
    {
        if (!_cardDiscarded)
        {
            _cardDiscarded = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial12-09".ToLocalized()));
        }
    }

    protected override void Execute(BattleStateChanged msg) => _turn = msg.State.TurnNumber;

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Thoughts/Tutorial12-01",
            "Thoughts/Tutorial12-01_Controller",
            "Thoughts/Tutorial12-02",
            "Thoughts/Tutorial12-02_Controller",
            "Thoughts/Tutorial12-03",
            "Thoughts/Tutorial12-04",
            "Thoughts/Tutorial12-05",
            "Thoughts/Tutorial12-06",
            "Thoughts/Tutorial12-07",
            "Thoughts/Tutorial12-08",
            "Thoughts/Tutorial12-09",
        };
}