using System.Collections;
using UnityEngine;

public class Tutorial11Orchestrator : OnMessage<StartCardSetupRequested, CardResolutionFinished, WinBattleWithRewards, BattleStateChanged, ShowCurrentTutorialAgain>, ILocalizeTerms
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
        yield return TutorialSettings.BattleTutorialPanelPopupDelay;
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
        // Outdated: No Longer Required since All-In does it all
        // if (msg.CardName == "Acid Coating")
        // {
        //     _playedAcidCoating = true;
        //     Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-01".ToLocalized()));
        // }
        if (msg.CardName == "Aegis")
        {
            _hasPlayedAegis = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-02".ToLocalized()));
        }
        else if (msg.CardName == "Erase" && !_hasPlayedAegis)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-03".ToLocalized()));
        }
        else if (msg.CardName == "Erase" && _hasPlayedAegis)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-04".ToLocalized()));
        }
        // Outdated: No Longer Required since All-In does it all
        // else if (!_hasShowedOutplayHint && _turn == 2 && msg.CardName == "All In" && !_playedAcidCoating)
        // {
        //     _hasShowedOutplayHint = true;
        //     Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-05".ToLocalized()));
        // }
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
    protected override void Execute(BattleStateChanged msg)
    {
        var s = msg.State;
        _turn = s.TurnNumber;
        if (s.Enemies.Length < 1)
            return;
        
        // Outdated: No Longer Required since All-In does it all
        //var aegisCount = s.Enemies[0].Member.Aegis();
        // if (_turn == 2 && aegisCount > 0 && !_turn2AegisShown)
        // {
        //     _turn2AegisShown = true;
        //     Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial11-06".ToLocalized()));
        // }
    }

    public string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/Tutorial11-01",
        "Thoughts/Tutorial11-02",
        "Thoughts/Tutorial11-03",
        "Thoughts/Tutorial11-04",
        "Thoughts/Tutorial11-05",
        "Thoughts/Tutorial11-06",
    };
}
