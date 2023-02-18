using System.Linq;
using UnityEngine;

public class AchievementWatcher : MonoBehaviour
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private BattleState battleState;
    [SerializeField] private ProgressionProgress progress;
    
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
    }

    private bool IsNotTutorial => adventure.Adventure.id != AdventureIds.TutorialAdventureId;
    
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
