using System.Collections;
using UnityEngine;

public class Tutorial3Orchestrator : OnMessage<StartCardSetupRequested, TurnStarted, CardResolutionFinished, WinBattleWithRewards>
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
        yield return new WaitForSeconds(1);
        if (!_hasWon)
            Message.Publish(new ShowTutorialByName(_callerId));
    }

    protected override void Execute(TurnStarted msg) => UpdateHealthTotals();

    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.MemberId == 1 && (msg.CardName == "Strike" || msg.CardName == "Charged Strike") && mageHealth == battleState.Members[4].State.Hp() && warriorHealth == battleState.Members[5].State.Hp())
        {
            Message.Publish(new ShowHeroBattleThought(4, "A wasted attack! If only you had used a different type of damage..."));
        }
        if (msg.MemberId == 1 && msg.CardName == "Strike" && mageHealth != battleState.Members[4].State.Hp() && !_hitWithPhysicalAttack)
        {
            _hitWithPhysicalAttack = true;
            Message.Publish(new ShowHeroBattleThought(4, $"Oof! I can't stop your {TextColors.PhysDamageColoredDark("physical damage")}"));
        }
        if (msg.MemberId == 1 && msg.CardName == "Charged Strike" && warriorHealth != battleState.Members[5].State.Hp() && _hitWithMagicAttack)
        {
            _hitWithMagicAttack = true;
            Message.Publish(new ShowHeroBattleThought(5, $"Oof! Your {TextColors.MagicDamageColoredDark("magic")} attacks hurt"));
        }
        var heroHealthChanged = heroHealth != battleState.Members[1].State.Hp();
        if (msg.MemberId == 4 && heroHealthChanged)
        {
            Message.Publish(new ShowHeroBattleThought(4, $"You have no clue how to resist my {TextColors.MagicDamageColoredDark("magic")} attacks"));
        }
        if (msg.MemberId == 4 && msg.CardName == "Scorch" && !heroHealthChanged && !_blockedWithResistanceAlready)
        {
            _blockedWithResistanceAlready = true;
            Message.Publish(new ShowHeroBattleThought(4, "You resisted my entire attack!"));
        }
        if (msg.MemberId == 5 && heroHealthChanged)
        {
            Message.Publish(new ShowHeroBattleThought(5, "Maybe next time bring some armor"));
        }
        if (msg.MemberId == 5 && msg.CardName == "Devastating Blow" && !heroHealthChanged && !_blockedWithArmorAlready)
        {
            _blockedWithArmorAlready = true;
            Message.Publish(new ShowHeroBattleThought(5, "Impressive... your armor is strong"));
        }
        UpdateHealthTotals();
    }

    protected override void Execute(WinBattleWithRewards msg) => _hasWon = true;

    private void UpdateHealthTotals()
    {
        heroHealth = battleState.Members[1].State.Hp();
        mageHealth = battleState.Members[4].State.Hp();
        warriorHealth = battleState.Members[5].State.Hp();
    }
}