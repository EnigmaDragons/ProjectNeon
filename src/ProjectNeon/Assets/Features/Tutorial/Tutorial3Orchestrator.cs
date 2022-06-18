using System.Collections;
using UnityEngine;

public class Tutorial3Orchestrator : OnMessage<StartCardSetupRequested, TurnStarted, CardResolutionFinished>
{
    private const string _callerId = "Tutorial3Orchestrator";

    [SerializeField] private BattleState battleState;

    private int heroHealth;
    private int mageHealth;
    private int warriorHealth;
    
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
        Message.Publish(new ShowTutorialByName(_callerId));
    }

    protected override void Execute(TurnStarted msg) => UpdateHealthTotals();

    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.MemberId == 1 && (msg.CardName == "Strike" || msg.CardName == "Charged Strike") && mageHealth == battleState.Members[4].State.Hp() && warriorHealth == battleState.Members[5].State.Hp())
        {
            Message.Publish(new ShowHeroBattleThought(4, "A wasted attack! If only you had used a different type of damage..."));
        }
        if (msg.MemberId == 4 && heroHealth != battleState.Members[1].State.Hp())
        {
            Message.Publish(new ShowHeroBattleThought(4, "You have no clue how to resist my magic attacks"));
        }
        if (msg.MemberId == 5 && heroHealth != battleState.Members[1].State.Hp())
        {
            Message.Publish(new ShowHeroBattleThought(5, "Maybe next time bring some armor"));
        }
        UpdateHealthTotals();
    }

    private void UpdateHealthTotals()
    {
        heroHealth = battleState.Members[1].State.Hp();
        mageHealth = battleState.Members[4].State.Hp();
        warriorHealth = battleState.Members[5].State.Hp();
    }
}