using UnityEngine;

public abstract class AdventureProgressBase : ScriptableObject
{
    public abstract int AdventureId { get; }
    public abstract string AdventureName { get; }
    public abstract GameAdventureProgressType AdventureType { get; }
    public abstract int RngSeed { get; }
    public abstract bool UsesRewardXp { get; }
    public abstract float BonusXpLevelFactor { get; }
    public abstract bool IsFinalStageSegment { get; }
    public abstract int CurrentStageProgress { get; }
    public abstract int CurrentChapterNumber { get; }
    public abstract int CurrentPowerLevel { get; }
    public abstract int CurrentElitePowerLevel { get; }
    public abstract IStage Stage { get; }
    public abstract CurrentGlobalEffects GlobalEffects { get; }
    public abstract void Reset();
    public abstract void InitIfNeeded();
    public abstract void Init(Adventure adventure, int chapterIndex);
    public abstract void Init(Adventure adventure, int chapterIndex, int segmentIndex); // Technically this breaks LSP. V2 Progress doesn't exactly use Segments.
    public abstract void AdvanceStageIfNeeded();
    public abstract LootPicker CreateLootPicker(PartyAdventureState party);
    public abstract GameAdventureProgressData GetData();
    public abstract void Advance();
    public abstract void SetStoryState(string state, bool value);
    public abstract bool IsTrue(string state);
    public abstract bool IsFinalBoss { get; }
    public abstract float ProgressToBoss { get; }
    public abstract float[] RisingActionPoints { get; }
}
