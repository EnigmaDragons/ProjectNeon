using UnityEngine;

public abstract class AdventureProgressBase : ScriptableObject
{
    public abstract bool UsesRewardXp { get; }
    public abstract float BonusXpLevelFactor { get; }
    public abstract bool IsFinalStageSegment { get; }
    public abstract int CurrentChapterNumber { get; }
    public abstract int CurrentPowerLevel { get; }
    public abstract int CurrentElitePowerLevel { get; }
    public abstract IStage Stage { get; }
    public abstract CurrentGlobalEffects GlobalEffects { get; }
    public abstract void Reset();
    public abstract void InitIfNeeded();
    public abstract void Init(Adventure adventure, int chapterIndex);
    public abstract void AdvanceStageIfNeeded();
    public abstract LootPicker CreateLootPicker(PartyAdventureState party);
    public abstract GameAdventureProgressData GetData();
    public abstract void Advance();
}