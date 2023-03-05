using System.Collections;
using UnityEngine;

public class Tutorial3Orchestrator : OnMessage<StartCardSetupRequested, TurnStarted, CardResolutionFinished, WinBattleWithRewards, MemberUnconscious, ShowCurrentTutorialAgain>, ILocalizeTerms
{
    private const string _callerId = "Tutorial3Orchestrator";

    [SerializeField] private BattleState battleState;

    private int heroHealth;
    private int mageHealth;
    private int warriorHealth;
    private bool _hitWithPhysicalAttack;
    private bool _hitWithMagicAttack;
    private bool _blockedWithArmorAlready;
    private bool _blockedWithResistanceAlready;
    private bool _hasWon;
    private bool _hasMadeUnconsciousAllyComment;
    
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
        ShowTutorial();
    }
    
    protected override void Execute(ShowCurrentTutorialAgain msg) => ShowTutorial();

    private void ShowTutorial()
    {
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }


    protected override void Execute(TurnStarted msg) => UpdateHealthTotals();

    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.MemberId == 1 && (msg.CardName == "Strike" || msg.CardName == "Charged Strike") && mageHealth == battleState.Members[4].State.Hp() && warriorHealth == battleState.Members[5].State.Hp())
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial03-01".ToLocalized()));
        }
        if (msg.MemberId == 1 && msg.CardName == "Strike" && mageHealth != battleState.Members[4].State.Hp() && !_hitWithPhysicalAttack)
        {
            _hitWithPhysicalAttack = true;
            Message.Publish(new ShowHeroBattleThought(4, string.Format("Thoughts/Tutorial03-02".ToLocalized(), TextColors.PhysDamageColoredDark("Thoughts/Tutorial03-03".ToLocalized()))));
        }
        if (msg.MemberId == 1 && msg.CardName == "Charged Strike" && warriorHealth != battleState.Members[5].State.Hp() && _hitWithMagicAttack)
        {
            _hitWithMagicAttack = true;
            Message.Publish(new ShowHeroBattleThought(5, string.Format("Thoughts/Tutorial03-04".ToLocalized(), TextColors.MagicDamageColoredDark("Thoughts/Tutorial03-05".ToLocalized()))));
        }
        var heroHealthChanged = heroHealth != battleState.Members[1].State.Hp();
        if (msg.MemberId == 4 && heroHealthChanged)
        {
            Message.Publish(new ShowHeroBattleThought(4, string.Format("Thoughts/Tutorial03-06".ToLocalized(), TextColors.MagicDamageColoredDark("Thoughts/Tutorial03-07".ToLocalized()))));
        }
        if (msg.MemberId == 4 && msg.CardName == "Scorch" && !heroHealthChanged && !_blockedWithResistanceAlready)
        {
            _blockedWithResistanceAlready = true;
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial03-08".ToLocalized()));
        }
        if (msg.MemberId == 5 && heroHealthChanged)
        {
            Message.Publish(new ShowHeroBattleThought(5, "Thoughts/Tutorial03-09".ToLocalized()));
        }
        if (msg.MemberId == 5 && msg.CardName == "Devastating Blow" && !heroHealthChanged && !_blockedWithArmorAlready)
        {
            _blockedWithArmorAlready = true;
            Message.Publish(new ShowHeroBattleThought(5, "Thoughts/Tutorial03-10".ToLocalized()));
        }
        UpdateHealthTotals();
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;
    
    protected override void Execute(MemberUnconscious msg)
    {
        if (_hasMadeUnconsciousAllyComment)
            return;

        _hasMadeUnconsciousAllyComment = true;
        if (msg.Member.Id == 4)
        {
            Message.Publish(new ShowHeroBattleThought(5, "Thoughts/Tutorial03-11".ToLocalized()));
        }
        if (msg.Member.Id == 5)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Thoughts/Tutorial03-12".ToLocalized()));
        }
    }

    private void UpdateHealthTotals()
    {
        heroHealth = battleState.Members[1].State.Hp();
        mageHealth = battleState.Members[4].State.Hp();
        warriorHealth = battleState.Members[5].State.Hp();
    }

    public string[] GetLocalizeTerms() => new[]
    {
        "Thoughts/Tutorial03-01",
        "Thoughts/Tutorial03-02",
        "Thoughts/Tutorial03-03",
        "Thoughts/Tutorial03-04",
        "Thoughts/Tutorial03-05",
        "Thoughts/Tutorial03-06",
        "Thoughts/Tutorial03-07",
        "Thoughts/Tutorial03-08",
        "Thoughts/Tutorial03-09",
        "Thoughts/Tutorial03-10",
        "Thoughts/Tutorial03-11",
        "Thoughts/Tutorial03-12",
    };
}