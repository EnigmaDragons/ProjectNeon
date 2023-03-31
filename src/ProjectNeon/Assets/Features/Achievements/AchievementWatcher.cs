using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementWatcher : MonoBehaviour
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private BattleState battleState;
    [SerializeField] private ProgressionProgress progress;

    private readonly HashSet<int> _consciousEnemiesOnCardResolutionStart = new HashSet<int>();
    
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }

    private void OnEnable()
    {
        Message.Subscribe<MemberStateChanged>(OnMemberStateChanged, this);
        Message.Subscribe<CardPurchased>(OnCardPurchased, this);
        Message.Subscribe<HealedInjury>(_ => Achievements.Record(Achievement.MiscHealedInjury), this);
        Message.Subscribe<SkipCutsceneRequested>(_ => Achievements.Record(Achievement.MiscSkippedCutscene), this);
        Message.Subscribe<GameProgressUpdated>(OnProgressUpdated, this);
        Message.Subscribe<BattleFinished>(OnBattleFinished, this);
        Message.Subscribe<BattleStateChanged>(OnBattleStateChanged, this);
        Message.Subscribe<CardResolutionStarted>(OnCardResolutionStarted, this);
        Message.Subscribe<CardResolutionFinished>(OnCardResolutionFinished, this);
    }

    private void OnBattleStateChanged(BattleStateChanged msg)
    {
        var turnAdvanced = msg.Before.TurnNumber != msg.State.TurnNumber;
        var notTutorialAndTurnAdvanced = IsNotTutorial && turnAdvanced;
        if (notTutorialAndTurnAdvanced && msg.Before.PlayedCardHistory.AnyNonAlloc() && msg.Before.PlayedCardHistory.Last().Count(c => c.Card.Speed != CardSpeed.Quick) >= 5)
            Achievements.Record(Achievement.PlaystyleFiveCardPlaysInATurn);
        if (turnAdvanced && msg.State.Heroes.AnyNonAlloc(h => h.IsConscious() && h.CurrentShield() >= 60))
            Achievements.Record(Achievement.PlaystyleShields60);
        if (turnAdvanced && msg.State.Heroes.AnyNonAlloc(h => h.IsConscious() && h.PrimaryResourceAmount() >= 32))
            Achievements.Record(Achievement.PlaystyleResources32);
        if (msg.State.PlayerState.NumberOfCyclesUsedThisTurn >= 10)
            Achievements.Record(Achievement.PlaystyleCycle10);
    }

    private void OnCardResolutionFinished(CardResolutionFinished msg)
    {
        var beforeConsciousCount = _consciousEnemiesOnCardResolutionStart.Count;
        var afterSameMemberConsciousCount = battleState.EnemyMembers.Count(x =>
            x.IsConscious() && _consciousEnemiesOnCardResolutionStart.Contains(x.Id));
        var numberMadeUnconscious = beforeConsciousCount - afterSameMemberConsciousCount;
        if (IsNotTutorial && numberMadeUnconscious >= 2)
            Achievements.Record(Achievement.CombatMultiKill);
        _consciousEnemiesOnCardResolutionStart.Clear();
    }

    private void OnCardResolutionStarted(CardResolutionStarted obj)
    {
        _consciousEnemiesOnCardResolutionStart.Clear();
        battleState.ConsciousEnemyMembers.ForEach(m => _consciousEnemiesOnCardResolutionStart.Add(m.Id));
    }

    private bool IsNotTutorial => adventure == null || adventure.Adventure == null || adventure.Adventure.id != AdventureIds.TutorialAdventureId;

    private void OnBattleFinished(BattleFinished msg)
    {
        if (msg.Winner == TeamType.Party && battleState.Heroes.Length > 1 && battleState.Heroes.Count(x => x.IsConscious()) == 1)
            Achievements.Record(Achievement.CombatLastOneStanding);
        if (IsNotTutorial && msg.Winner == TeamType.Party && battleState.TurnNumber == 1)
            Achievements.Record(Achievement.CombatFirstTurnVictory);
    }

    private void OnProgressUpdated(GameProgressUpdated msg)
    {
        var completedFactor = progress.GetProgressCompletionFactor();
        if (completedFactor >= 0.5f)
            Achievements.Record(Achievement.Progress50Percent);
        if (completedFactor >= 1f)
            Achievements.Record(Achievement.Progress100Percent);
    }

    private void OnCardPurchased(CardPurchased msg)
    {
        if (IsNotTutorial && msg.Card.Rarity == Rarity.Epic)
            Achievements.Record(Achievement.MiscBoughtAnEpicCard);
    }
    
    private void OnMemberStateChanged(MemberStateChanged msg)
    {
        if (battleState.Members.TryGetValue(msg.MemberId(), out var m) 
                && m.TeamType == TeamType.Enemies 
                && msg.BeforeState.HpAndShield -42 == msg.State.HpAndShield())
            Achievements.Record(Achievement.Combat42Damage);
        
        if (msg.WasKnockedOut() && msg.BeforeState.Hp >= 200)
            Achievements.Record(Achievement.PlaystyleOneShot);
    }
}
