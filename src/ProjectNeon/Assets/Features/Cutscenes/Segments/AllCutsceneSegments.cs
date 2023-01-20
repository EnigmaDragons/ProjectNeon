using System;
using System.Collections.Generic;

public static class AllCutsceneSegments
{
    private static readonly Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        CreateSegmentOfType = new Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        {
            {CutsceneSegmentType.Nothing, e => new SimpleSegment(() => { })},
            {CutsceneSegmentType.DialogueLine, e => new MessagePublishSegment(
                new ShowCharacterDialogueLine(e.DialogueCharacterId, e.Term.ToLocalized()), 
                new FullyDisplayDialogueLine(e.DialogueCharacterId))},
            {CutsceneSegmentType.NarratorLine, e => new MessagePublishSegment(
                new ShowCharacterDialogueLine(CutsceneCharacterAliases.Narrator, e.Term.ToLocalized()),
                new FullyDisplayDialogueLine(CutsceneCharacterAliases.Narrator))},
            {CutsceneSegmentType.Wait, e => new MessagePublishSegment(new CutsceneWaitRequested(e.FloatAmount))},
            {CutsceneSegmentType.Choice, e => new MessagePublishSegment(new BeginStoryEvent2(e.StoryEvent))},
            {CutsceneSegmentType.MultiChoice, e => new MessagePublishSegment(new BeginStoryEvent2(e.StoryEvent))},
            {CutsceneSegmentType.RecordStoryState, e => new MessagePublishSegment(new RecordStoryStateRequested(e.StoryState.Value))},
            {CutsceneSegmentType.HideCharacter, e => new MessagePublishSegment(new HideCharacterRequested(e.DialogueCharacterId.Value))},
            {CutsceneSegmentType.ShowCharacter, e => new MessagePublishSegment(new ShowCharacterRequested(e.DialogueCharacterId.Value))},
            {CutsceneSegmentType.PlayerLine, e => new MessagePublishSegment(
                new ShowCharacterDialogueLine(CutsceneCharacterAliases.Player, e.Term.ToLocalized()),
                new FullyDisplayDialogueLine(CutsceneCharacterAliases.Player))},
            { CutsceneSegmentType.ActivateGlitchEffect, e => new MessagePublishSegment(new CutsceneGlitchEffectRequested(true))},
            { CutsceneSegmentType.DeactivateGlitchEffect, e => new MessagePublishSegment(new CutsceneGlitchEffectRequested(false))},
            { CutsceneSegmentType.FadeOut, e => new MessagePublishSegment(new CutsceneFadeRequested(false, e.FloatAmount > 0 ? e.FloatAmount : 2f)) },
            { CutsceneSegmentType.FadeIn, e => new MessagePublishSegment(new CutsceneFadeRequested(true, e.FloatAmount > 0 ? e.FloatAmount : 2f)) },
            { CutsceneSegmentType.TriggerCutsceneEvent, e => new MessagePublishSegment(new TriggerCutsceneEvent(e.CutsceneEventName, e.FloatAmount > 0 ? e.FloatAmount : 2f))}
        };

    public static CutsceneSegment Create(CutsceneSegmentData data)
    {
        var type = data.SegmentType;
        try
        {
            if (!CreateSegmentOfType.ContainsKey(type))
            {
                Log.Error($"No SegmentType of {type} exists in {nameof(AllCutsceneSegments)}");
#if UNITY_EDITOR
                throw new KeyNotFoundException($"Construction Details not found for Segment Type {type}");
#endif
                return CreateSegmentOfType[CutsceneSegmentType.Nothing](data);
            }

            return CreateSegmentOfType[type](data);
        }
        catch (Exception e)
        {
            Log.Error($"SegmentType {type} is broken {e}");
#if UNITY_EDITOR
            throw;
#endif
            return CreateSegmentOfType[CutsceneSegmentType.Nothing](data);
        }
    }
}
